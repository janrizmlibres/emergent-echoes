using EmergentEchoes.Entities.Actors;
using EmergentEchoes.Utilities.Components.Records;
using EmergentEchoes.Utilities.Events;
using Godot;
using System;
using System.Collections.Generic;

namespace EmergentEchoes
{
	public partial class WorldState : Node
	{
		private static readonly Lazy<WorldState> _instance = new(() => new WorldState());

		public static WorldState Instance { get { return _instance.Value; } }

		private WorldState() { }

		private readonly List<Actor> _actors = new();

		private readonly List<Node2D> _structures = new();

		// Actions and interactions that have taken place and what actors performed them
		private readonly List<Event> _globalRecords = new();

		// Which crimes are publicly known and not yet solved
		private readonly List<Crime> _unsolvedCrimes = new();

		// Solved crimes and who apprehended the criminal
		private readonly List<Crime> _solvedCrimes = new();

		// What tasks are delegated to which individuals
		private readonly List<Event> _taskRecords = new();

		// Workplaces and who they belong to (Possibly _structures)
		// Current date (?)

		// TODO: Consider limiting access to data

		public List<Actor> GetActors()
		{
			return _actors;
		}

		public void AddActor(Actor actor)
		{
			_actors.Add(actor);
		}
	}
}