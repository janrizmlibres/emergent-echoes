using Godot;
using NPCProcGen.Autoloads;

namespace EmergentEchoes.Stages.Island
{
	public partial class World : Node2D
	{
		public override void _Ready()
		{
			AutoloadInitializer.Init(this);
		}
	}
}