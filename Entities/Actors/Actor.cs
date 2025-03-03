using EmergentEchoes.Entities.Hooks.CarryProp;
using EmergentEchoes.Utilities.Game;
using Godot;
using NPCProcGen;

namespace EmergentEchoes.Entities.Actors
{
	public partial class Actor : CharacterBody2D
	{
		[Signal]
		public delegate void CropPlantedEventHandler(CropMarker2D cropMarker);
		[Signal]
		public delegate void CropHarvestedEventHandler(CropMarker2D cropMarker);

		[Export]
		public int MaxSpeed { get; set; } = 40;
		[Export]
		public int Acceleration { get; set; } = 8;
		[Export]
		public int Friction { get; set; } = 4;

		protected AnimationTree _animationTree;
		protected AnimationNodeStateMachinePlayback _animationState;

		protected Sprite2D _seedProp;
		protected CarryProp _carryProp;

		protected EmoteController _emoteController;
		protected FloatTextController _floatTextController;

		public override void _Ready()
		{
			_animationTree = GetNode<AnimationTree>("AnimationTree");
			_animationState = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");

			_seedProp = GetNode<Sprite2D>("SeedProp");
			_carryProp = GetNode<CarryProp>("CarryProp");

			_emoteController = GetNode<EmoteController>("EmoteController");
			_floatTextController = GetNode<FloatTextController>("FloatTextController");
		}
	}
}