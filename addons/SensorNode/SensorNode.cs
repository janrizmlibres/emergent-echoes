#if TOOLS
using Godot;
using System;

namespace EmergentEchoes
{
	[Tool]
	public partial class SensorNode : EditorPlugin
	{
		public override void _EnterTree()
		{
			// Initialization of the plugin goes here.
			Script script = GD.Load<Script>("res://addons/SensorNode/Sensor.cs");
			Texture2D texture = GD.Load<Texture2D>("res://addons/SensorNode/Icon.png");
			AddCustomType("Sensor", "Node", script, texture);
		}

		public override void _ExitTree()
		{
			// Clean-up of the plugin goes here.
			RemoveCustomType("Sensor");
		}
	}
}
#endif