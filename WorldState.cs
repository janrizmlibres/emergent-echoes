using Godot;
using System;
using System.Collections.Generic;

namespace EmergentEchoes
{
	public partial class WorldState : Node
	{
		public static WorldState Instance { get; private set; }

		public List<CharacterBody2D> NPCList { get; private set; }

		public override void _Ready()
		{
			Instance = this;
			NPCList = new List<CharacterBody2D>();
		}
	}
}
