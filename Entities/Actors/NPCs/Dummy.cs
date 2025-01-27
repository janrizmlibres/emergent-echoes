using Godot;
using System;
using Godot.Collections;
using System.Linq;
using NPCProcGen;

namespace EmergentEchoes.Entities.Actors
{
    public partial class Dummy : CharacterBody2D
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
        private TileMapLayer _tileMapLayer;
        private NPCAgent2D _npcAgent2D; // ! Remove after testing

        private State _state = State.Idle;

        private readonly Array<Vector2I> _validTilePositions = new();

        public override void _Ready()
        {
            if (Engine.IsEditorHint()) return;

            _stateTimer = GetNode<Timer>("StateTimer");
            _navigationAgent2d = GetNode<NavigationAgent2D>("NavigationAgent2D");
            _animationTree = GetNode<AnimationTree>("AnimationTree");
            _animationState = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");

            _stateTimer.Timeout += OnNavigationFinished;
            _stateTimer.OneShot = true;
            _stateTimer.Start(GetRandRange());

            _navigationAgent2d.NavigationFinished += OnNavigationFinished;
            _navigationAgent2d.VelocityComputed += OnNavigationAgentVelocityComputed;

            // ! Remove after testing
            _npcAgent2D = GetNode<NPCAgent2D>("NPCAgent2D");
            _npcAgent2D.PetitionStarted += OnPetitionStarted;
            _npcAgent2D.InteractionEnded += () => SetPhysicsProcess(true);

            _tileMapLayer = GetNode<TileMapLayer>("%TileMapLayer");
            SetupTilePositions();
        }

        private static int GetRandRange()
        {
            return GD.RandRange(1, 3);
        }

        public override void _PhysicsProcess(double delta)
        {
            if (Engine.IsEditorHint()) return;

            if (_state == State.Idle)
            {
                IdleState();
            }
            else if (_state == State.Wander)
            {
                MoveCharacter();
            }

            HandleAnimation();
            MoveAndSlide();
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
            State[] values = Enum.GetValues(typeof(State)).Cast<State>().ToArray();
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

        private void OnNavigationFinished()
        {
            ChangeState();
        }

        // ! Remove after testing
        private void OnPetitionStarted(Vector2 directionToFace)
        {
            _animationTree.Set("parameters/Idle/blend_position", directionToFace.X);
            _animationState.Travel("Idle");
            SetPhysicsProcess(false);
        }

        private void ChangeState()
        {
            _state = RandomizeState();

            switch (_state)
            {
                case State.Idle:
                    _stateTimer.Start(GetRandRange());
                    break;
                case State.Wander:
                    Vector2 wanderTarget = PickTargetPosition();
                    _navigationAgent2d.TargetPosition = wanderTarget;
                    break;
            }
        }
    }
}