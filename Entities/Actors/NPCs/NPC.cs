using Godot;
using Godot.Collections;
using NPCProcGen;
using NPCProcGen.Core.Components.Enums;
using EmergentEchoes.Utilities.Game.Enums;
using EmergentEchoes.Utilities;
using EmergentEchoes.Stages.Testing;
using System.Collections.Generic;
using System;

namespace EmergentEchoes.Entities.Actors
{
    public partial class NPC : Actor
    {
        private enum MainState { Idle, Wander, Procedural, Eat, Harvest, Attack }

        private const float MinStateInterval = 3;
        private const float MaxStateInterval = 5;

        private readonly List<MainState> _passiveStates = new()
        {
            MainState.Idle,
            MainState.Wander
        };

        private MainState _mainState = MainState.Idle;
        private int _maxSpeed;
        private int _acceleration;

        private readonly Array<Vector2I> _validTilePositions = new();

        private WorldLayer _worldLayer;
        private NavigationAgent2D _navigationAgent2d;
        private NPCAgent2D _npcAgent2d;
        private Timer _stateTimer;

        public override void _Ready()
        {
            base._Ready();

            _maxSpeed = MaxSpeed;
            _acceleration = Acceleration;

            _worldLayer = GetNode<WorldLayer>("%WorldLayer");
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
            _npcAgent2d.InteractionStarted += OnInteractionStarted;
            _npcAgent2d.InteractionEnded += OnInteractionEnded;
            _npcAgent2d.EventTriggered += OnEventTriggered;

            SetupTilePositions();
        }

        private void SetupTilePositions()
        {
            Array<Vector2I> usedCells = _worldLayer.GetUsedCells();

            foreach (Vector2I cell in usedCells)
            {
                TileData tileData = _worldLayer.GetCellTileData(cell);

                if (tileData != null && (bool)tileData.GetCustomData("isNavigatable"))
                {
                    _validTilePositions.Add(cell);
                }
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            if (Engine.IsEditorHint()) return;

            switch (_mainState)
            {
                case MainState.Idle:
                    StopMoving();
                    break;
                case MainState.Wander:
                    HandleWanderState();
                    break;
                case MainState.Procedural:
                    HandleProceduralState();
                    break;
                case MainState.Eat:
                    _navigationAgent2d.Velocity = Velocity.MoveToward(Vector2.Zero, Friction);
                    _animationState.Travel("Eat");
                    break;
            }

            MoveAndSlide();
        }

        public void ShowFloatText(ResourceType resType, string text)
        {
            _floatTextController.ShowFloatText(resType, text);
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
                StopMoving();
                return;
            }

            MoveCharacter();
        }

        private void StopMoving()
        {
            _navigationAgent2d.Velocity = Velocity.MoveToward(Vector2.Zero, Friction);
            _animationState.Travel("Idle");
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
                _animationTree.Set("parameters/Idle/blend_position", Velocity.X);
                _animationTree.Set("parameters/Move/blend_position", Velocity.X);
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
            if (_validTilePositions.Count > 0)
            {
                int randomIdx = (int)(GD.Randi() % _validTilePositions.Count);
                Vector2I chosenCell = _validTilePositions[randomIdx];
                return _worldLayer.MapToLocal(chosenCell);
            }

            return Vector2.Zero;
        }

        private void OnAnimationFinished(StringName animName)
        {
            if (animName.ToString().Contains("eat"))
            {
                _npcAgent2d.CompleteConsumption();
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
            CropMarker2D cropMarker = data[0].As<CropMarker2D>();
            _seedProp.Visible = false;
            EmitSignal(nameof(CropPlanted), cropMarker);
            _npcAgent2d.CompletePlanting();
        }

        private void OnHarvestEntered(Array<Variant> data)
        {
            CropMarker2D cropMarker = data[0].As<CropMarker2D>();
            // Play harvest animation
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
                    _carryProp.Visible = false;
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
            Node2D partner = data[0].As<Node2D>();
            NPC npc = partner as NPC;

            float companionshipIncrease = data[1].As<float>();

            npc?.ShowFloatText(ResourceType.Companionship, companionshipIncrease.ToString());
            _floatTextController.ShowFloatText(
                ResourceType.Companionship,
                companionshipIncrease.ToString()
            );
        }

        private void OnInteractionStarted(Variant state, Array<Variant> data)
        {
            Node2D partner = data[0].As<Node2D>();
            FacePartner(partner);

            _worldLayer.Obstacles.Add(this);
            _worldLayer.NotifyRuntimeTileDataUpdate();
        }

        private void FacePartner(Node2D partner)
        {
            Vector2 directionToFace = GlobalPosition.DirectionTo(partner.GlobalPosition);

            _animationTree.Set("parameters/Idle/blend_position", directionToFace.X);
            _animationState.Travel("Idle");

            _mainState = MainState.Idle;
            _emoteController.Activate();
        }

        private void OnInteractionEnded()
        {
            _mainState = MainState.Procedural;
            _emoteController.Deactivate();

            _worldLayer.Obstacles.Remove(this);
            _worldLayer.NotifyRuntimeTileDataUpdate();
        }

        private void OnEventTriggered(Variant @event, Array<Variant> data)
        {
            EventType eventType = @event.As<EventType>();

            switch (eventType)
            {
                case EventType.CrimeWitnessed:
                    _emoteController.ShowEmoteBubble(Emote.Interrobang);
                    break;
                case EventType.Detained:
                    OnDetained();
                    break;
                case EventType.Captured:
                    OnCaptured(data);
                    break;
            }
        }

        private void OnDetained()
        {
            Visible = false;
            _stateTimer.Stop();
        }

        private void OnCaptured(Array<Variant> data)
        {
            Vector2 releaseLocation = data[0].As<Vector2>();
            GlobalPosition = releaseLocation;
            Visible = true;
            RandomizeMainState();
        }
    }
}
