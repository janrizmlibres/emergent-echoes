using EmergentEchoes.Utilities.Game.Enums;
using Godot;
using Godot.Collections;
using NPCProcGen;
using NPCProcGen.Core.Components.Enums;

namespace EmergentEchoes.Entities.Actors
{
	public partial class Player : Actor
	{
		public enum State { Active, Dormant }

		private State _state = State.Active;

		private ActorTag2D _actorTag2D;

		public override void _Ready()
		{
			base._Ready();

			_actorTag2D = GetNode<ActorTag2D>("ActorTag2D");
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
			Node2D partner = data[0].As<Node2D>();
			Vector2 directionToFace = GlobalPosition.DirectionTo(partner.GlobalPosition);

			_animationTree.Set("parameters/Idle/blend_position", directionToFace.X);
			_animationState.Travel("Idle");

			_state = State.Dormant;
			_emoteController.Activate();
		}

		private void OnInteractionStartedOnNPCAlt(string state, Node2D npc)
		{
			if (state == "petitioning")
			{
				_emoteController.ShowEmoteBubble(0);
				npc.GetNode("Blackboard").Call("set_value", "current_state", "petition answered");
			}

			Vector2 directionToFace = GlobalPosition.DirectionTo(npc.GlobalPosition);

			_animationTree.Set("parameters/Idle/blend_position", directionToFace.X);
			_animationState.Travel("Idle");

			_state = State.Dormant;
		}

		private void OnInteractionEnded()
		{
			_state = State.Active;
			_emoteController.Deactivate();
		}

		private void OnEventTriggered(Variant eventType, Array<Variant> data)
		{
			EventType type = eventType.As<EventType>();

			if (type == EventType.CrimeWitnessed)
			{
				_emoteController.ShowEmoteBubble(Emote.Interrobang);
				return;
			}

			if (type == EventType.Detained)
			{
				// Immobilize
			}
		}
	}
}
