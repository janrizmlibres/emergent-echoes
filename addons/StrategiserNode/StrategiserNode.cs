#if TOOLS
using Godot;
using System;

namespace EmergentEchoes
{
	[Tool]
	public partial class StrategiserNode : EditorPlugin
	{
		public override void _EnterTree()
		{
			Script script = GD.Load<Script>("res://addons/StrategiserNode/Strategiser.cs");
			Texture2D texture = GD.Load<Texture2D>("res://addons/StrategiserNode/Icon.png");
			AddCustomType("Strategiser", "Node", script, texture);
		}

		public override void _ExitTree()
		{
			// Clean-up of the plugin goes here.
			RemoveCustomType("Strategiser");
		}
	}
}
#endif
