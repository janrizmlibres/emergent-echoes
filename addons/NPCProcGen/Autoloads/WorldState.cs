using Godot;
using NPCProcGen.Core.Components.Records;
using NPCProcGen.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NPCProcGen
{
	public partial class WorldState : Node
	{
		private static readonly Lazy<WorldState> _instance = new(() => new WorldState());

		public static WorldState Instance { get { return _instance.Value; } }

		private WorldState() { }

		public List<ActorTag2D> Actors { get; private set; } = new();

		private readonly List<Node2D> _structures = new();
		// Actions and interactions that have taken place and what actors performed them
		private readonly List<Event> _globalEvents = new();
		// Which crimes are publicly known and not yet solved
		private readonly List<Crime> _unsolvedCrimes = new();
		// Solved crimes and who apprehended the criminal
		private readonly List<Crime> _solvedCrimes = new();
		// What tasks are delegated to which individuals
		private readonly List<Event> _taskRecords = new();

		// Workplaces and who they belong to (Possibly _structures)
		// Current date (?)

		// TODO: Consider limiting access to data

		public void Initialize(Node2D currentScene)
		{
			FindActorsInNode(currentScene);

			// ! Remove print debug loop in production
			GD.Print("Actors in World State:");
			foreach (ActorTag2D actor in Actors)
			{
				GD.Print(actor.Parent.Name);
			}

			foreach (ActorTag2D actor in Actors)
			{
				if (actor is NPCAgent2D npc)
				{
					npc.Initialize(Actors.Where(a => a != actor).ToList());
				}
			}
		}

		private void FindActorsInNode(Node node)
		{
			foreach (Node child in node.GetChildren())
			{
				if (child is ActorTag2D actor)
				{
					Actors.Add(actor);
				}

				FindActorsInNode(child);
			}
		}
	}
}