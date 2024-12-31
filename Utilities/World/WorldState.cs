using EmergentEchoes.Entities.Actors;
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

		// TODO: Consider limiting access to actors

		public List<Actor> GetActors()
		{
			return _actors;
		}

		public void AddActor(Actor actor)
		{
			_actors.Add(actor);
		}

		public void RemoveActor(Actor actor)
		{
			_actors.Remove(actor);
		}
	}
}