#if TOOLS
using Godot;
using System;

namespace NPCProcGen
{
	[Tool]
	public partial class NPCAgent2DNode : EditorPlugin
	{
		public override void _EnterTree()
		{
			Script npcAgent2DScript = GD.Load<Script>("res://addons/NPCProcGen/Nodes/NPCAgent2D/NPCAgent2D.cs");
			Texture2D npcAgent2DTexture = GD.Load<Texture2D>("res://addons/NPCProcGen/Nodes/NPCAgent2D/Icon.png");
			AddCustomType("NPCAgent2D", "Node", npcAgent2DScript, npcAgent2DTexture);

			Script actorTag2DScript = GD.Load<Script>("res://addons/NPCProcGen/Nodes/ActorTag2D/ActorTag2D.cs");
			Texture2D actorTag2DTexture = GD.Load<Texture2D>("res://addons/NPCProcGen/Nodes/ActorTag2D/Icon.png");
			AddCustomType("ActorTag2D", "Node", actorTag2DScript, actorTag2DTexture);
		}

		public override void _ExitTree()
		{
			RemoveCustomType("NPCAgent2D");
			RemoveCustomType("ActorTag2D");
		}

		public override void _EnablePlugin()
		{
			AddAutoloadSingleton("WorldState", "res://addons/NPCProcGen/Autoloads/WorldState.cs");
		}

		public override void _DisablePlugin()
		{
			RemoveAutoloadSingleton("WorldState");
		}
	}
}
#endif
