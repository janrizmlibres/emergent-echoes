using Godot;
using System;

namespace EmergentEchoes
{
	public partial class Player : CharacterBody2D
	{
		[Export] private int MaxSpeed { get; set; } = 80;
		[Export] private int Acceleration { get; set; } = 10;
		[Export] private int Friction { get; set; } = 10;

		private AnimationTree _animationTree;
		private AnimationNodeStateMachinePlayback _animationState;

		public override void _Ready()
		{
			_animationTree = GetNode<AnimationTree>("AnimationTree");
			_animationState = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");
		}

		public override void _PhysicsProcess(double delta)
		{
			Vector2 inputVector = Input.GetVector("left", "right", "up", "down");

			if (inputVector.X != 0)
			{
				_animationTree.Set("parameters/Idle/blend_position", inputVector.X);
				_animationTree.Set("parameters/Move/blend_position", inputVector.X);
				Move(inputVector);
			}
			else if (inputVector.Y != 0)
			{
				Move(inputVector);
			}
			else
			{
				_animationState.Travel("Idle");
				Velocity = Velocity.MoveToward(Vector2.Zero, Friction);
			}

			MoveAndSlide();
		}

		private void Move(Vector2 inputVector)
		{
			_animationState.Travel("Move");
			Velocity = Velocity.MoveToward(inputVector * MaxSpeed, Acceleration);
		}
	}
}