using EmergentEchoes.Utilities.Game;
using EmergentEchoes.Utilities.Game.Enums;
using Godot;
using Godot.Collections;
using NPCProcGen;
using NPCProcGen.Core.Components.Enums;

namespace EmergentEchoes.Entities.Actors
{
	public partial class Player : CharacterBody2D
	{
		public enum State { Active, Dormant }

		[Export] private int MaxSpeed { get; set; } = 80;
		[Export] private int Acceleration { get; set; } = 10;
		[Export] private int Friction { get; set; } = 10;

		private State _state = State.Active;

		private TileMapLayer _tileMapLayer;

		private AnimationTree _animationTree;
		private AnimationNodeStateMachinePlayback _animationState;
		private ActorTag2D _actorTag2D;
		private EmoteController _emoteController;

		public override void _Ready()
		{
			_tileMapLayer = GetNode<TileMapLayer>("%WorldLayer");

			_animationTree = GetNode<AnimationTree>("AnimationTree");
			_animationState = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");
			_actorTag2D = GetNode<ActorTag2D>("ActorTag2D");
			_emoteController = GetNode<EmoteController>("EmoteController");

			_actorTag2D.InteractionStarted += OnInteractionStarted;
			_actorTag2D.InteractionEnded += OnInteractionEnded;
			_actorTag2D.EventTriggered += OnEventTriggered;
		}

		public override void _PhysicsProcess(double delta)
		{
			switch (_state)
			{
				case State.Active:
					HandleActiveState();
					break;
				case State.Dormant:
					StopMoving();
					break;
			}
		}

		private void HandleActiveState()
		{
			Vector2 inputVector = Input.GetVector("left", "right", "up", "down");

			if (inputVector.X != 0)
			{
				_animationTree.Set("parameters/Idle/blend_position", inputVector.X);
				_animationTree.Set("parameters/Move/blend_position", inputVector.X);
				MovePlayer(inputVector);
			}
			else if (inputVector.Y != 0)
			{
				MovePlayer(inputVector);
			}
			else
			{
				_animationState.Travel("Idle");
				StopMoving();
			}

			MoveAndSlide();
		}

		private void StopMoving()
		{
			Velocity = Velocity.MoveToward(Vector2.Zero, Friction);
		}

		private void MovePlayer(Vector2 inputVector)
		{
			_animationState.Travel("Move");
			Velocity = Velocity.MoveToward(inputVector * MaxSpeed, Acceleration);
		}

		private void OnInteractionStarted(Variant state, Array<Variant> data)
		{
			InteractState actionState = state.As<InteractState>();

			if (actionState == InteractState.Petition)
			{
				_actorTag2D.AnswerPetition(true);
			}

			Node2D partner = data[0].As<Node2D>();
			Vector2 directionToFace = GlobalPosition.DirectionTo(partner.GlobalPosition);

			_animationTree.Set("parameters/Idle/blend_position", directionToFace.X);
			_animationState.Travel("Idle");

			_state = State.Dormant;
			_emoteController.Activate();
		}

		private void OnInteractionEnded()
		{
			_state = State.Active;
			_emoteController.Deactivate();
		}

		private void OnEventTriggered(Variant eventType)
		{
			EventType type = eventType.As<EventType>();

			if (type == EventType.CrimeWitnessed)
			{
				_emoteController.ShowEmoteBubble(Emote.Interrobang);
			}
			else if (type == EventType.Captured)
			{
				// Captured
			}
			else if (type == EventType.Released)
			{
				// Released
			}
		}
	}
}
