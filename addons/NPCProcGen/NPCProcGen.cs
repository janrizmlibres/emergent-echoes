#if TOOLS
using Godot;

namespace NPCProcGen
{
	[Tool]
	public partial class NPCProcGen : EditorPlugin
	{
		public override void _EnterTree()
		{
			RegisterCustomType(
				"res://addons/NPCProcGen/Nodes/NPCAgent2D/NPCAgent2D.cs",
				"res://addons/NPCProcGen/Nodes/NPCAgent2D/Icon.png",
				"Node",
				"NPCAgent2D"
			);

			RegisterCustomType(
				"res://addons/NPCProcGen/Nodes/ActorTag2D/ActorTag2D.cs",
				"res://addons/NPCProcGen/Nodes/ActorTag2D/Icon.png",
				"Node",
				"ActorTag2D"
			);

			RegisterCustomType(
				"res://addons/NPCProcGen/Nodes/ShopMarker2D/ShopMarker2D.cs",
				"res://addons/NPCProcGen/Nodes/ShopMarker2D/Icon.png",
				"Marker2D",
				"ShopMarker2D"
			);

			RegisterCustomType(
				"res://addons/NPCProcGen/Nodes/CropMarker2D/CropMarker2D.cs",
				"res://addons/NPCProcGen/Nodes/CropMarker2D/Icon.png",
				"Marker2D",
				"CropMarker2D"
			);

			RegisterCustomType(
				"res://addons/NPCProcGen/Nodes/PrisonArea2D/PrisonArea2D.cs",
				"res://addons/NPCProcGen/Nodes/PrisonArea2D/Icon.png",
				"Area2D",
				"PrisonArea2D"
			);
		}

		private void RegisterCustomType(string scriptPath, string iconPath, string baseName, string name)
		{
			Script script = GD.Load<Script>(scriptPath);
			Texture2D texture = GD.Load<Texture2D>(iconPath);
			AddCustomType(name, baseName, script, texture);
		}

		public override void _ExitTree()
		{
			RemoveCustomType("NPCAgent2D");
			RemoveCustomType("ActorTag2D");
			RemoveCustomType("ShopMarker2D");
			RemoveCustomType("CropMarker2D");
			RemoveCustomType("PrisonArea2D");
		}

		public override void _EnablePlugin()
		{
			// AddAutoloadSingleton("Initializer", "res://addons/NPCProcGen/Autoloads/AutoloadInitializer.cs");
		}

		public override void _DisablePlugin()
		{
			// RemoveAutoloadSingleton("Initializer");
		}
	}
}
#endif
