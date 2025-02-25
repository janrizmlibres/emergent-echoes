using Godot;

namespace EmergentEchoes.Entities.Hooks
{
	public partial class EvaluationTimer : Timer
	{
		private const int MinWaitTime = 10;
		private const int MaxWaitTime = 20;

		public void Start()
		{
			Start(GD.RandRange(MinWaitTime, MaxWaitTime));
		}
	}
}