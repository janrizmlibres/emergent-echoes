using EmergentEchoes.Entities.Actors;
using Godot;
using System;

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
		}

		public override void _Process(double delta)
		{
		}
	}
}