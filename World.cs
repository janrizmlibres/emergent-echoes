using Godot;
using System;

using Godot.Collections;

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

			foreach (CharacterBody2D actor in WorldState.Instance.NPCList)
			{
				GD.Print(actor.Name);
			}
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}
	}
}
