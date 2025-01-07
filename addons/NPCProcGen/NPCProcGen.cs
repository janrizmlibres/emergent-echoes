#if TOOLS
using Godot;

namespace NPCProcGen
{
	[Tool]
	public partial class NPCProcGen : EditorPlugin
	{
		private string _npcAgent2DName = "NPCAgent2D";
		private string _actorTag2DName = "ActorTag2D";
		private string _autoloadName = "WorldState";

		public override void _EnterTree()
		{
			RegisterCustomType(
				"res://addons/NPCProcGen/Nodes/NPCAgent2D/NPCAgent2D.cs",
				"res://addons/NPCProcGen/Nodes/NPCAgent2D/Icon.png",
				_npcAgent2DName
			);

			RegisterCustomType(
				"res://addons/NPCProcGen/Nodes/ActorTag2D/ActorTag2D.cs",
				"res://addons/NPCProcGen/Nodes/ActorTag2D/Icon.png",
				_actorTag2DName
			);
		}

		private void RegisterCustomType(string scriptPath, string iconPath, string name)
		{
			Script script = GD.Load<Script>(scriptPath);
			Texture2D texture = GD.Load<Texture2D>(iconPath);
			AddCustomType(name, "Node", script, texture);
		}

		public override void _ExitTree()
		{
			RemoveCustomType(_npcAgent2DName);
			RemoveCustomType(_actorTag2DName);
		}

		public override void _EnablePlugin()
		{
			AddAutoloadSingleton(_autoloadName, "res://addons/NPCProcGen/Autoloads/WorldState.cs");
		}

		public override void _DisablePlugin()
		{
			RemoveAutoloadSingleton(_autoloadName);
		}
	}
}
#endif
