using Godot;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NPCProcGen.Autoloads
{
	/// <summary>
	/// Represents the global state of the world, including actors, structures, events, and crimes.
	/// </summary>
	public partial class WorldState : Node
	{
		public WorldState()
		{
			DebugTool.Assert(!_isInstantiated, "WorldState has already been instantiated");
			_isInstantiated = true;
		}

		~WorldState()
		{
			_isInstantiated = false;
		}

		/// <summary>
		/// Gets the list of actors in the world.
		/// </summary>
		public List<ActorTag2D> Actors => _actors.ToList();

		private bool _isInstantiated = false;

		private List<ActorTag2D> _actors;
		private List<PrisonArea2D> _prisons = new();

		private readonly Dictionary<ActorTag2D, ActorState> _actorState = new();
		private readonly Dictionary<NPCAgent2D, Crime> _investigations = new();

		/// <summary>
		/// Actions and interactions that have taken place and what actors performed them.
		/// </summary>
		private readonly List<string> _globalEvents = new();

		/// <summary>
		/// Crimes that are publicly known and not yet solved.
		/// </summary>
		private readonly Queue<Crime> _recordedCrimes = new();
		private readonly List<Crime> _unsolvedCrimes = new();
		private readonly List<Crime> _solvedCrimes = new();

		/// <summary>
		/// List of structures in the world.
		/// </summary>
		private readonly List<Node2D> _structures = new();

		// ? Workplaces and who they belong to (Possibly _structures)
		// ? Current date (?)

		// TODO: Consider limiting access to data

		/// <summary>
		/// Initializes the world state with the given list of actors.
		/// </summary>
		/// <param name="actors">The list of actors to initialize.</param>
		public void Initialize(List<ActorTag2D> actors, List<PrisonArea2D> prisons)
		{
			_actors = actors;
			_prisons = prisons;

			foreach (ActorTag2D actor in actors)
			{
				actor.Initialize(actors.Where(a => actor != a).ToList());
				_actorState.Add(actor, new ActorState());
			}
		}

		public Tuple<ActionType, ActionState> GetTaskRecord(ActorTag2D actor)
		{
			DebugTool.Assert(_actorState.ContainsKey(actor), "Actor does not have an action record");
			return _actorState[actor].CurrentTask ?? null;
		}

		public void SetTaskRecord(ActorTag2D actor, ActionType actionType, ActionState actionState)
		{
			DebugTool.Assert(_actorState.ContainsKey(actor), "Actor does not have an action record");
			_actorState[actor].CurrentTask = new Tuple<ActionType, ActionState>(actionType, actionState);
		}

		public void ResetTaskRecord(ActorTag2D actor)
		{
			DebugTool.Assert(_actorState.ContainsKey(actor), "Actor does not have an action record");
			_actorState[actor].CurrentTask = null;
		}

		public bool IsActorBusy(ActorTag2D actor)
		{
			Tuple<ActionType, ActionState> action = GetTaskRecord(actor);

			if (action == null) return false;

			ActionState state = action.Item2;

			return state == ActionState.Talk || state == ActionState.Petition
				|| state == ActionState.Interact || state == ActionState.Flee
				|| state == ActionState.Capture;
			// || state == ActionState.Eat || state == ActionState.Research
		}

		public ResourceType? GetPetitionResourceType(ActorTag2D actor)
		{
			DebugTool.Assert(_actorState.ContainsKey(actor), "Actor does not have an action record");
			return _actorState[actor].CurrentPetitionResourceType;
		}

		public void SetPetitionResourceType(ActorTag2D actor, ResourceType type)
		{
			DebugTool.Assert(_actorState.ContainsKey(actor), "Actor does not have an action record");
			_actorState[actor].CurrentPetitionResourceType = type;
		}

		public void ClearPetitionResourceType(ActorTag2D actor)
		{
			DebugTool.Assert(_actorState.ContainsKey(actor), "Actor does not have an action record");
			_actorState[actor].CurrentPetitionResourceType = null;
		}

		public bool HasCrimes()
		{
			return _recordedCrimes.Count > 0;
		}

		public void RecordCrime(Crime crime)
		{
			_recordedCrimes.Enqueue(crime);
		}

		public Crime InvestigateCrime(NPCAgent2D investigator)
		{
			if (_recordedCrimes.TryDequeue(out Crime crime))
			{
				crime.Investigator = investigator;
				_investigations.Add(investigator, crime);
				return crime;
			}
			return null;
		}

		public void CloseInvestigation(NPCAgent2D investigator)
		{
			DebugTool.Assert(
				_investigations.ContainsKey(investigator),
				"Investigator is not investigating a crime"
			);

			Crime crime = _investigations[investigator];
			_investigations.Remove(investigator);
			_unsolvedCrimes.Add(crime);
		}

		public void SolveInvestigation(NPCAgent2D investigator)
		{
			DebugTool.Assert(
				_investigations.ContainsKey(investigator),
				"Investigator is not investigating a crime"
			);

			Crime crime = _investigations[investigator];
			_investigations.Remove(investigator);
			_solvedCrimes.Add(crime);
		}

		public PrisonArea2D GetAvailablePrison()
		{
			return CommonUtils.Shuffle(_prisons).First();
		}
	}
}