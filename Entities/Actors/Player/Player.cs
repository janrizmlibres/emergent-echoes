using EmergentEchoes.Utilities;
using Godot;
using Godot.Collections;
using NPCProcGen;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.States;

namespace EmergentEchoes.Entities.Actors
{
	public partial class Player : CharacterBody2D
	{
		[Export] private int MaxSpeed { get; set; } = 80;
		[Export] private int Acceleration { get; set; } = 10;
		[Export] private int Friction { get; set; } = 10;

		public enum State { Active, Dormant }

		private State _state = State.Active;

		private AnimationTree _animationTree;
		private AnimationNodeStateMachinePlayback _animationState;
		private ActorTag2D _actorTag2D;

		public override void _Ready()
		{
			_animationTree = GetNode<AnimationTree>("AnimationTree");
			_animationState = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");

			_actorTag2D = GetNode<ActorTag2D>("ActorTag2D");
			_actorTag2D.InteractionStarted += OnInteractionStarted;
			_actorTag2D.InteractionEnded += OnInteractionEnded;
		}

		public override void _PhysicsProcess(double delta)
		{
			switch (_state)
			{
				case State.Active:
					MovePlayer();
					break;
			}

		}

		private void MovePlayer()
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

		private void OnInteractionStarted(Variant state, Array<Variant> data)
		{
			ActionState actionState = state.As<ActionState>();

			if (actionState is ActionState.Talk)
			{
				Node2D partner = data[0].As<Node2D>();
				Vector2 directionToFace = GlobalPosition.DirectionTo(partner.GlobalPosition);

				_animationTree.Set("parameters/Idle/blend_position", directionToFace.X);
				_animationState.Travel("Idle");

				_state = State.Dormant;
			}
		}

		private void OnInteractionEnded()
		{
			_state = State.Active;
		}
	}
}