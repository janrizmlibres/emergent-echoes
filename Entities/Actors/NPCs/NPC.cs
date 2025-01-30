using Godot;
using System;
using Godot.Collections;
using NPCProcGen;
using NPCProcGen.Core.Components.Enums;
using EmergentEchoes.Utilities;
using EmergentEchoes.Utilities.Enums;
using System.Linq;

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

        private enum MainState { Idle, Wander, Procedural }
        private enum ProceduralState { Active, Dormant }

        private MainState _mainState = MainState.Idle;
        private ProceduralState _proceduralState = ProceduralState.Active;

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

            _stateTimer = GetNode<Timer>("StateTimer");
            _navigationAgent2d = GetNode<NavigationAgent2D>("NavigationAgent2D");
            _animationTree = GetNode<AnimationTree>("AnimationTree");
            _animationState = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");
            _npcAgent2d = GetNode<NPCAgent2D>("NPCAgent2D");

            _stateTimer.Timeout += ChangeState;
            _stateTimer.OneShot = true;
            _stateTimer.Start(GD.RandRange(1.0, 3.0));

            _navigationAgent2d.VelocityComputed += OnNavigationAgentVelocityComputed;

            _npcAgent2d.ExecutionStarted += OnExecutionStarted;
            _npcAgent2d.ExecutionEnded += OnExecutionEnded;
            _npcAgent2d.ActionStateEntered += OnActionStateEntered;
            _npcAgent2d.ActionStateExited += OnActionStateExited;

            _tileMapLayer = GetNode<TileMapLayer>("%TileMapLayer");
            SetupTilePositions();
        }

        public override void _PhysicsProcess(double delta)
        {
            if (Engine.IsEditorHint()) return;

            HandleAnimation();
            MoveAndSlide();

            switch (_mainState)
            {
                case MainState.Idle:
                    IdleState();
                    break;
                case MainState.Wander:
                    MoveCharacter();
                    break;
                case MainState.Procedural:
                    ExecuteProcedural();
                    break;
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
                ChangeState();
                return;
            }

            Vector2 destination = _navigationAgent2d.GetNextPathPosition();
            Vector2 direction = GlobalPosition.DirectionTo(destination);
            _navigationAgent2d.Velocity = Velocity.MoveToward(direction * MaxSpeed, Acceleration);
        }

        private void ExecuteProcedural()
        {
            switch (_proceduralState)
            {
                case ProceduralState.Active:
                    ActiveProceduralState();
                    break;
            }
        }

        private void ActiveProceduralState()
        {
            _navigationAgent2d.TargetPosition = _npcAgent2d.TargetPosition;
            MoveCharacter();
        }

        private void MoveNavigationAgent()
        {
            if (_navigationAgent2d.IsNavigationFinished())
            {
                _npcAgent2d.CompleteNavigation();
                return;
            }

            Vector2 destination = _navigationAgent2d.GetNextPathPosition();
            Vector2 direction = GlobalPosition.DirectionTo(destination);
            _navigationAgent2d.Velocity = Velocity.MoveToward(direction * MaxSpeed, Acceleration);
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

        private static MainState RandomizeState()
        {
            MainState[] values = Enum.GetValues(typeof(MainState)).Cast<MainState>().ToArray();
            int randomIdx = GD.RandRange(0, values.Length - 1);
            return values[randomIdx];
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

        private void ChangeState()
        {
            _mainState = RandomizeState();

            switch (_mainState)
            {
                case MainState.Idle:
                    _stateTimer.Start(GD.RandRange(1.0, 3.0));
                    break;
                case MainState.Wander:
                    Vector2 wanderTarget = PickTargetPosition();
                    _navigationAgent2d.TargetPosition = wanderTarget;
                    break;
            }
        }

        private void OnExecutionStarted(Variant action)
        {
            ActionType actionType = action.As<ActionType>();

            if (actionType == ActionType.Theft)
            {
                EmoteManager.ShowEmoteBubble(this, Emote.Hum);
            }
        }

        private void OnExecutionEnded(Variant action)
        {
            ChangeState();
        }

        private void OnActionStateEntered(Variant state, Array<Variant> data)
        {
            ActionState actionState = state.As<ActionState>();

            if (actionState == ActionState.Steal)
            {
                EmoteManager.ShowEmoteBubble(this, Emote.Mark);
            }
            else if (actionState == ActionState.Talk || actionState == ActionState.Petition)
            {
                Node2D partner = data[0].As<Node2D>();
                Vector2 directionToFace = GlobalPosition.DirectionTo(partner.GlobalPosition);

                _animationTree.Set("parameters/Idle/blend_position", directionToFace.X);
                _animationState.Travel("Idle");

                _proceduralState = ProceduralState.Dormant;
            }
        }

        private void OnActionStateExited(Variant state, Array<Variant> data)
        {
            ActionState actionState = state.As<ActionState>();

            if (actionState == ActionState.Talk || actionState == ActionState.Petition)
            {
                _proceduralState = ProceduralState.Active;
            }
        }
    }
}