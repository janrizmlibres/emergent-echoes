#if TOOLS
using Godot;
using System;

namespace EmergentEchoes
{
	[Tool]
	public partial class MemoriserNode : EditorPlugin
	{
		public override void _EnterTree()
		{
			// Initialization of the plugin goes here.
			Script script = GD.Load<Script>("res://addons/MemoriserNode/Memoriser.cs");
			Texture2D texture = GD.Load<Texture2D>("res://addons/MemoriserNode/Icon.png");
			AddCustomType("Memoriser", "Node", script, texture);
		}

		public override void _ExitTree()
		{
			// Clean-up of the plugin goes here.
			RemoveCustomType("Memoriser");
		}
	}
}
#endif
