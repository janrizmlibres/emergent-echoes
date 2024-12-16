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
			// AddCustomType("Sensor", "Node", script);
		}

		public override void _ExitTree()
		{
			// Clean-up of the plugin goes here.
		}
	}
}
#endif