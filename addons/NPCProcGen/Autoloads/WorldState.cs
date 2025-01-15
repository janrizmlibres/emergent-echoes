using Godot;
using NPCProcGen.Core.Components.Records;
using NPCProcGen.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NPCProcGen.Autoloads
{
	public partial class WorldState : Node
	{
		private static readonly Lazy<WorldState> _instance = new(() => new WorldState());

		public static WorldState Instance { get { return _instance.Value; } }

		private WorldState() { }

		public IReadOnlyList<ActorTag2D> Actors { get; private set; }

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