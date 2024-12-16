using Godot;
using System;

namespace EmergentEchoes
{
	public partial class World : Node2D
	{
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			foreach (Node child in GetChildren())
			{
				if (child is CharacterBody2D actor)
				{
					WorldState.Instance.NPCList.Add(actor);
				}
			}
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}
	}
}