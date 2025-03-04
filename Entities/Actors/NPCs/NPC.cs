using Godot;
using NPCProcGen;
using NPCProcGen.Core.Components.Enums;
using EmergentEchoes.Utilities.Game.Enums;
using EmergentEchoes.Utilities;
using System.Collections.Generic;
using System;
using Godot.Collections;

namespace EmergentEchoes.Entities.Actors
{
    public partial class NPC : Actor
    {
        private enum MainState { Idle, Wander, Procedural, Eat, Harvest, Attack }

        private const float MinStateInterval = 3;
        private const float MaxStateInterval = 5;

        public Array<Vector2I> ValidTilePositions { get; private set; } = new();

        private readonly List<MainState> _passiveStates = new()
        {
            MainState.Idle,
            MainState.Wander
        };

        private MainState _mainState = MainState.Idle;
        private int _maxSpeed;
        private int _acceleration;

        private NavigationAgent2D _navigationAgent2d;
        private NPCAgent2D _npcAgent2d;
        private Timer _stateTimer;

        public override void _Ready()
        {
            base._Ready();

            _maxSpeed = MaxSpeed;
            _acceleration = Acceleration;

            _navigationAgent2d = GetNode<NavigationAgent2D>("NavigationAgent2D");
            _npcAgent2d = GetNode<NPCAgent2D>("NPCAgent2D");
            _stateTimer = GetNode<Timer>("StateTimer");

            _stateTimer.Timeout += RandomizeMainState;
            _stateTimer.Start(GD.RandRange(MinStateInterval, MaxStateInterval));

            _animationTree.AnimationFinished += OnAnimationFinished;
            _navigationAgent2d.VelocityComputed += OnNavigationAgentVelocityComputed;

            _npcAgent2d.ExecutionStarted += OnExecutionStarted;
            _npcAgent2d.ExecutionEnded += OnExecutionEnded;
            _npcAgent2d.ActionStarted += OnActionStarted;
            _npcAgent2d.StateEntered += OnStateEntered;
            _npcAgent2d.StateExited += OnStateExited;
        }

        public override void _PhysicsProcess(double delta)
        {
            if (Engine.IsEditorHint()) return;

            switch (_mainState)
            {
                case MainState.Idle:
                case MainState.Eat:
                case MainState.Harvest:
                case MainState.Attack:
                    StopMoving(_mainState.ToString());
                    break;
                case MainState.Wander:
                    HandleWanderState();
                    break;
                case MainState.Procedural:
                    HandleProceduralState();
                    break;
            }

            MoveAndSlide();
        }

        private void HandleWanderState()
        {
            if (_navigationAgent2d.IsNavigationFinished())
            {
                RandomizeMainState();
                return;
            }

            MoveCharacter();
        }

        private void HandleProceduralState()
        {
            _navigationAgent2d.TargetPosition = _npcAgent2d.TargetPosition;

            if (_navigationAgent2d.IsNavigationFinished())
            {
                _npcAgent2d.CompleteNavigation();
                StopMoving("Idle");
                return;
            }

            MoveCharacter();
        }

        private void StopMoving(StringName animName)
        {
            _navigationAgent2d.Velocity = Velocity.MoveToward(Vector2.Zero, Friction);
            _animationState.Travel(animName);
        }

        private void MoveCharacter()
        {
            Vector2 destination = _navigationAgent2d.GetNextPathPosition();
            Vector2 direction = GlobalPosition.DirectionTo(destination);
            _navigationAgent2d.Velocity = Velocity.MoveToward(direction * _maxSpeed, _acceleration);

            HandleAnimation();
        }

        private void HandleAnimation()
        {
            if (Velocity.X != 0)
            {
                SetBlendPositionParams(Velocity.X);
            }

            _animationState.Travel("Move");
        }

        private void RandomizeMainState()
        {
            _mainState = CoreHelpers.ShuffleList(_passiveStates)[0];

            switch (_mainState)
            {
                case MainState.Idle:
                    _stateTimer.Start(GD.RandRange(MinStateInterval, MaxStateInterval));
                    break;
                case MainState.Wander:
                    Vector2 wanderTarget = PickTargetPosition();
                    _navigationAgent2d.TargetPosition = wanderTarget;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Vector2 PickTargetPosition()
        {
            if (ValidTilePositions.Count > 0)
            {
                int randomIdx = (int)(GD.Randi() % ValidTilePositions.Count);
                Vector2I chosenCell = ValidTilePositions[randomIdx];
                return WorldLayer.MapToLocal(chosenCell);
            }

            return Vector2.Zero;
        }

        private void OnAnimationFinished(StringName animName)
        {
            switch (animName.ToString())
            {
                case string name when name.Contains("eat"):
                    _npcAgent2d.CompleteConsumption();
                    break;
                case string name when name.Contains("harvest"):
                    _npcAgent2d.CompleteHarvest();
                    break;
            }
        }

        private void OnNavigationAgentVelocityComputed(Vector2 safeVelocity)
        {
            Velocity = safeVelocity;
        }

        private void OnExecutionStarted()
        {
            _mainState = MainState.Procedural;
            _stateTimer.Stop();
        }

        private void OnExecutionEnded()
        {
            _maxSpeed = MaxSpeed;
            _acceleration = Acceleration;
            RandomizeMainState();
        }

        private void OnActionStarted(Variant action)
        {
            ActionType actionType = action.As<ActionType>();

            switch (actionType)
            {
                case ActionType.Theft:
                    _emoteController.ShowEmoteBubble(Emote.Hum);
                    break;
                case ActionType.Eat:
                    _mainState = MainState.Eat;
                    break;
            }
        }

        private void OnStateEntered(Variant state, Array<Variant> data)
        {
            ActionState actionState = state.As<ActionState>();

            _maxSpeed = MaxSpeed;
            _acceleration = Acceleration;

            switch (actionState)
            {
                case ActionState.Capture:
                    OnCaptureEntered(data);
                    break;
                case ActionState.FindTile:
                    OnFindTileEntered();
                    break;
                case ActionState.Plant:
                    OnPlantEntered(data);
                    break;
                case ActionState.Harvest:
                    OnHarvestEntered(data);
                    break;
                case ActionState.Assess:
                    _emoteController.Activate();
                    break;
                case ActionState.Engage:
                    _maxSpeed = MaxSpeed + 10;
                    _acceleration = Acceleration + 2;
                    break;
            }
        }

        private void OnCaptureEntered(Array<Variant> data)
        {
            StringName actorName = data[0].As<StringName>();
            _carryProp.SetTexture(actorName);
            _carryProp.Visible = true;
        }

        private void OnFindTileEntered()
        {
            _seedProp.Visible = true;
        }

        private void OnPlantEntered(Array<Variant> data)
        {
            _seedProp.Visible = false;

            CropMarker2D cropMarker = data[0].As<CropMarker2D>();
            EmitSignal(Actor.SignalName.CropPlanted, cropMarker);
            _npcAgent2d.CompletePlanting();
        }

        private void OnHarvestEntered(Array<Variant> data)
        {
            CropMarker2D cropMarker = data[0].As<CropMarker2D>();
            EmitSignal(Actor.SignalName.CropHarvested, cropMarker);
            _mainState = MainState.Harvest;
        }

        private void OnStateExited(Variant state, Array<Variant> data)
        {
            ActionState actionState = state.As<ActionState>();

            switch (actionState)
            {
                case ActionState.Petition:
                    OnPetitionExited(data);
                    break;
                case ActionState.Steal:
                    OnStealExited(data);
                    break;
                case ActionState.Wander:
                    OnWanderExited(data);
                    break;
                case ActionState.Eat:
                    OnEatExited(data);
                    break;
                case ActionState.Talk:
                    OnTalkExited(data);
                    break;
                case ActionState.Capture:
                    OnCaptureExited(data);
                    break;
                case ActionState.Assess:
                    _emoteController.Deactivate();
                    break;
                case ActionState.Engage:
                    _maxSpeed = MaxSpeed;
                    _acceleration = Acceleration;
                    break;
            }
        }

        private void OnPetitionExited(Array<Variant> data)
        {
            bool isAccepted = data[0].As<bool>();

            if (isAccepted)
            {
                ResourceType type = data[1].As<ResourceType>();
                int amountPetitioned = data[2].As<int>();
                _floatTextController.ShowFloatText(type, amountPetitioned.ToString());
                return;
            }

            int companionshipChange = data[1].As<int>();

            _floatTextController.ShowFloatText(
                ResourceType.Companionship,
                companionshipChange.ToString()
            );
        }

        private void OnStealExited(Array<Variant> data)
        {
            float amountStolen = data[1].As<float>();
            _floatTextController.ShowFloatText(ResourceType.Money, amountStolen.ToString(), false);
        }

        private void OnWanderExited(Array<Variant> data)
        {
            bool durationReached = data[0].As<bool>();
            if (!durationReached) return;
            _emoteController.ShowEmoteBubble(Emote.Ellipsis);
        }

        private void OnEatExited(Array<Variant> data)
        {
            int satiationIncrease = data[0].As<int>();

            _floatTextController.ShowFloatText(
                ResourceType.Satiation,
                satiationIncrease.ToString()
            );
        }

        private void OnTalkExited(Array<Variant> data)
        {
            float companionshipIncrease = data[1].As<float>();

            _floatTextController.ShowFloatText(
                ResourceType.Companionship,
                companionshipIncrease.ToString()
            );
        }

        private void OnCaptureExited(Array<Variant> data)
        {
            int dutyIncrease = data[0].As<int>();
            _carryProp.Visible = false;
            _floatTextController.ShowFloatText(ResourceType.Duty, dutyIncrease.ToString());
        }

        protected override void ExecuteInteractionStarted() => _mainState = MainState.Idle;
        protected override void ExecuteInteractionEnded() => _mainState = MainState.Procedural;

        protected override void ExecuteDetained() => _stateTimer.Stop();
        protected override void ExecuteCaptured() => RandomizeMainState();
    }
}
