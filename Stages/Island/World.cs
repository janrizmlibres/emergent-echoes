using EmergentEchoes.addons.NPC2DNode;
using EmergentEchoes.Entities.Actors;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EmergentEchoes
{
	public partial class World : Node2D
	{
		public override void _Ready()
		{
			foreach (Node child in GetChildren())
			{
				if (child is Actor actor)
				{
					WorldState.Instance.AddActor(actor);
				}
			}

			List<Actor> actors = WorldState.Instance.GetActors();

			foreach (Actor actor in actors)
			{
				List<Actor> others = actors.Where(a => a != actor).ToList();

				if (actor is Player player)
				{
					player.AddRelationships(others);
				}
				else if (actor is NPC2D npc)
				{
					npc.AddRelationships(others);
				}
				else
				{
					throw new Exception("Actor type not recognized.");
				}
			}
		}
	}
}