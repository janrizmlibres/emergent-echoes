using Godot;

namespace EmergentEchoes.Entities.Actors
{
	public partial class Player : Actor
	{
		public enum State { Active, Dormant }

		private State _state = State.Active;

		public override void _Ready()
		{
			base._Ready();
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

			if (inputVector.Length() > 0)
			{
				if (inputVector.X != 0)
				{
					SetBlendPositionParams(inputVector.X);
				}

				MovePlayer(inputVector);
			}
			else
			{
				StopMoving();
			}

			MoveAndSlide();
		}

		private void StopMoving()
		{
			Velocity = Velocity.MoveToward(Vector2.Zero, Friction);
			_animationState.Travel("Idle");
		}

		private void MovePlayer(Vector2 inputVector)
		{
			Velocity = Velocity.MoveToward(inputVector * MaxSpeed, Acceleration);
			_animationState.Travel("Move");
		}

		protected override void ExecuteInteractionStarted() => _state = State.Dormant;
		protected override void ExecuteInteractionEnded() => _state = State.Active;

		protected override void ExecuteDetained() => CollisionMask = 0b0001;
		protected override void ExecuteCaptured() => CollisionMask = 0b0011;

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
	}
}
