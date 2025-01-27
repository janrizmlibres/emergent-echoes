using Godot;
using NPCProcGen.Core.Components.Records;
using NPCProcGen.Core.Events;
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
		private static readonly Lazy<WorldState> _instance = new(() => new WorldState());

		/// <summary>
		/// Gets the singleton instance of the WorldState.
		/// </summary>
		public static WorldState Instance { get { return _instance.Value; } }

		private WorldState() { }

		/// <summary>
		/// Gets the list of actors in the world.
		/// </summary>
		public IReadOnlyList<ActorTag2D> Actors { get; private set; }

		/// <summary>
		/// List of structures in the world.
		/// </summary>
		private readonly List<Node2D> _structures = new();

		/// <summary>
		/// Actions and interactions that have taken place and what actors performed them.
		/// </summary>
		private readonly List<Event> _globalEvents = new();

		/// <summary>
		/// Crimes that are publicly known and not yet solved.
		/// </summary>
		private readonly List<Crime> _unsolvedCrimes = new();

		/// <summary>
		/// Solved crimes and who apprehended the criminal.
		/// </summary>
		private readonly List<Crime> _solvedCrimes = new();

		/// <summary>
		/// Tasks that are delegated to which individuals.
		/// </summary>
		private readonly List<Event> _taskRecords = new();

		// ? Workplaces and who they belong to (Possibly _structures)
		// ? Current date (?)

		// TODO: Consider limiting access to data

		/// <summary>
		/// Initializes the world state with the given list of actors.
		/// </summary>
		/// <param name="actors">The list of actors to initialize.</param>
		public void Initialize(List<ActorTag2D> actors)
		{
			Actors = actors;

			foreach (ActorTag2D actor in actors)
			{
				if (actor is NPCAgent2D npc)
				{
					npc.Initialize(actors.Where(a => npc != a).ToList());
				}
			}
		}
	}
}