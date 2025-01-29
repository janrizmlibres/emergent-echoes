using Godot;
using System;
using Godot.Collections;
using NPCProcGen;
using NPCProcGen.Core.Components.Enums;
using EmergentEchoes.Utilities;
using EmergentEchoes.Utilities.Enums;
using NPCProcGen.Core.Components.Variants;

namespace EmergentEchoes.Entities.Actors
{
	public partial class NPC : CharacterBody2D
	{
		[Export]
		public int MaxSpeed { get; set; } = 60;
		[Export]
		public int Acceleration { get; set; } = 4;
		[Export]
		public int Friction { get; set; } = 4;

		private enum State { Idle, Wander }

		private Timer _stateTimer;
		private NavigationAgent2D _navigationAgent2d;
		private AnimationTree _animationTree;
		private AnimationNodeStateMachinePlayback _animationState;
		private NPCAgent2D _npcAgent2d;
		private TileMapLayer _tileMapLayer;

		// private State _state = State.Idle;
		private readonly Array<Vector2I> _validTilePositions = new();

		public override void _Ready()
		{
			if (Engine.IsEditorHint()) return;

			// _stateTimer = GetNode<Timer>("StateTimer");
			_navigationAgent2d = GetNode<NavigationAgent2D>("NavigationAgent2D");
			_animationTree = GetNode<AnimationTree>("AnimationTree");
			_animationState = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");
			_npcAgent2d = GetNode<NPCAgent2D>("NPCAgent2D");

			// _stateTimer.Timeout += OnNavigationFinished;
			// _stateTimer.OneShot = true;
			// _stateTimer.Start(GD.RandRange(1.0, 3.0));

			_navigationAgent2d.NavigationFinished += OnNavigationFinished;
			_navigationAgent2d.VelocityComputed += OnNavigationAgentVelocityComputed;

			_npcAgent2d.ExecutionStarted += OnExecutionStarted;
			_npcAgent2d.ActionStateEntered += OnActionStateEntered;
			_npcAgent2d.TheftCompleted += OnTheftCompleted;

			_tileMapLayer = GetNode<TileMapLayer>("%TileMapLayer");

			SetupTilePositions();
		}

		public override void _PhysicsProcess(double delta)
		{
			if (Engine.IsEditorHint()) return;

			if (_npcAgent2d.IsActive())
			{
				_navigationAgent2d.TargetPosition = _npcAgent2d.TargetPosition;
			}

			if (_navigationAgent2d.IsNavigationFinished())
			{
				_navigationAgent2d.Velocity = Velocity.MoveToward(Vector2.Zero, Friction);
				return;
			}

			Vector2 destination = _navigationAgent2d.GetNextPathPosition();
			Vector2 direction = GlobalPosition.DirectionTo(destination);
			_navigationAgent2d.Velocity = Velocity.MoveToward(direction * MaxSpeed, Acceleration);

			HandleAnimation();
			MoveAndSlide();

			// if (_state == State.Idle)
			// {
			//     IdleState();
			// }
			// else if (_state == State.Wander)
			// {
			//     MoveCharacter();
			// }
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

		private void HandleAnimation()
		{
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
		}

		private void IdleState()
		{
			_navigationAgent2d.Velocity = Velocity.MoveToward(Vector2.Zero, Friction);
		}

		private void MoveCharacter()
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

		private void OnNavigationFinished()
		{
			_npcAgent2d.CompleteNavigation();
			ChangeState();
		}

		private void ChangeState()
		{
			// _state = RandomizeState();

			// switch (_state)
			// {
			//     case State.Idle:
			//         _stateTimer.Start(GD.RandRange(1.0, 3.0));
			//         break;
			//     case State.Wander:
			//         Vector2 wanderTarget = PickTargetPosition();
			//         _navigationAgent2d.TargetPosition = wanderTarget;
			//         break;
			// }
		}

		private void OnExecutionStarted(Variant action)
		{
			ActionType actionType = action.As<ActionType>();

			if (actionType == ActionType.Theft)
			{
				EmoteManager.ShowEmoteBubble(this, Emote.Hum);
			}
		}

		private void OnActionStateEntered(Variant state)
		{
			ActionState actionState = state.As<ActionState>();

			if (actionState == ActionState.Steal)
			{
				EmoteManager.ShowEmoteBubble(this, Emote.Mark);
			}
		}

		private void OnTheftCompleted(TheftData theftData)
		{
			FloatTextManager.ShowFloatText(this, theftData.Amount.ToString());
		}
	}
}
