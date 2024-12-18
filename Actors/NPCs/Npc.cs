using Godot;
using Godot.Collections;
using System;

namespace EmergentEchoes
{
	public partial class Npc : CharacterBody2D
	{
		private Timer _stateTimer;
		private NavigationAgent2D _navigationAgent2d;
		private AnimationTree _animationTree;
		private AnimationNodeStateMachinePlayback _animationState;

		private enum State { Idle, Wander }

		private const int MaxSpeed = 60;
		private const int Acceleration = 4;
		private const int Friction = 4;

		private State _state = State.Idle;
		private TileMapLayer _tileMapLayer;
		private Array<Vector2I> _validTilePositions = new();

		public override void _Ready()
		{
			_stateTimer = GetNode<Timer>("StateTimer");
			_navigationAgent2d = GetNode<NavigationAgent2D>("NavigationAgent2D");
			_animationTree = GetNode<AnimationTree>("AnimationTree");
			_animationState = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");

			_stateTimer.Timeout += OnChangeState;
			_stateTimer.OneShot = true;
			_stateTimer.Start(GD.RandRange(1.0, 3.0));

			_navigationAgent2d.NavigationFinished += OnChangeState;
			_navigationAgent2d.VelocityComputed += OnNavigationAgentVelocityComputed;
			_navigationAgent2d.AvoidanceEnabled = true;
			_navigationAgent2d.PathPostprocessing = NavigationPathQueryParameters2D.PathPostProcessing.Edgecentered;
			_navigationAgent2d.DebugEnabled = true;

			_tileMapLayer = GetParent().GetNode<TileMapLayer>("TileMapLayer");
			SetupTilePositions();
		}

		private void SetupTilePositions()
		{
			Array<Vector2I> usedCells = _tileMapLayer.GetUsedCells();

			foreach (Vector2I cell in usedCells)
			{
				TileData tileData = _tileMapLayer.GetCellTileData(cell);

				if (tileData != null && (bool)tileData.GetCustomData("isNavigatable"))
				{
					_validTilePositions.Add(cell);
				}
			}
		}

		public override void _PhysicsProcess(double delta)
		{
			switch (_state)
			{
				case State.Idle:
					IdleState();
					break;
				case State.Wander:
					WanderState();
					break;
			}

			if (Velocity.X != 0)
			{
				_animationTree.Set("parameters/Idle/blend_position", Velocity.X);
				_animationTree.Set("parameters/Move/blend_position", Velocity.X);
				_animationState.Travel("Move");
			}
			else if (Velocity.Y != 0)
			{
				_animationState.Travel("Move");
			}
			else
			{
				_animationState.Travel("Idle");
			}

			MoveAndSlide();
		}

		private void IdleState()
		{
			_navigationAgent2d.Velocity = Velocity.MoveToward(Vector2.Zero, Friction);
		}

		private void WanderState()
		{
			if (_navigationAgent2d.IsNavigationFinished())
			{
				_navigationAgent2d.Velocity = Velocity.MoveToward(Vector2.Zero, Friction);
			}

			Vector2 destination = _navigationAgent2d.GetNextPathPosition();
			Vector2 direction = GlobalPosition.DirectionTo(destination);
			_navigationAgent2d.Velocity = Velocity.MoveToward(direction * MaxSpeed, Acceleration);
		}

		private static State RandomizeState()
		{
			System.Array values = Enum.GetValues(typeof(State));
			int randomIdx = GD.RandRange(0, values.Length - 1);
			return ((State[])values)[randomIdx];
		}

		private Vector2 PickTargetPosition()
		{
			if (_validTilePositions.Count > 0)
			{
				int randomIdx = (int)(GD.Randi() % _validTilePositions.Count);
				Vector2I chosenCell = _validTilePositions[randomIdx];
				return _tileMapLayer.MapToLocal(chosenCell);
			}

			return Vector2.Zero;
		}

		private void OnNavigationAgentVelocityComputed(Vector2 safeVelocity)
		{
			Velocity = safeVelocity;
		}

		private void OnChangeState()
		{
			_state = RandomizeState();

			switch (_state)
			{
				case State.Idle:
					_stateTimer.Start(GD.RandRange(1.0, 3.0));
					break;
				case State.Wander:
					Vector2 wanderTarget = PickTargetPosition();
					_navigationAgent2d.TargetPosition = wanderTarget;
					break;
			}
		}
	}
}