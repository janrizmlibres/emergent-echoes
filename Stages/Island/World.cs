using Godot;
using NPCProcGen;

namespace EmergentEchoes
{
	public partial class World : Node2D
	{
		public override void _Ready()
		{
			WorldState.Instance.Initialize(this);
		}
	}
}