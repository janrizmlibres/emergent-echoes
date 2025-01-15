using Godot;
using NPCProcGen.Autoloads;

namespace EmergentEchoes
{
	public partial class World : Node2D
	{
		public override void _Ready()
		{
			AutoloadInitializer.Init(this);
		}
	}
}