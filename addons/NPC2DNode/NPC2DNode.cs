#if TOOLS
using Godot;
using System;

namespace EmergentEchoes.addons.NPCNode
{
	[Tool]
	public partial class NPC2DNode : EditorPlugin
	{
		public override void _EnterTree()
		{
			Script script = GD.Load<Script>("res://addons/NPC2DNode/NPC2D.cs");
			Texture2D texture = GD.Load<Texture2D>("res://addons/NPC2DNode/Icon.png");
			AddCustomType("NPC2D", "CharacterBody2D", script, texture);
		}

		public override void _ExitTree()
		{
			RemoveCustomType("NPC2D");
		}
	}
}
#endif
