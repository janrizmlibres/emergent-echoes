using Godot;
using Godot.Collections;
using NPCProcGen;
using NPCProcGen.Core.Components.Enums;
using System.Linq;
using EmergentEchoes.Utilities.Game.Enums;
using EmergentEchoes.Utilities.Game;
using EmergentEchoes.Utilities;
using EmergentEchoes.Stages.Testing;

namespace EmergentEchoes.Entities.Actors
{
    public partial class NPC : CharacterBody2D
    {
        private enum MainState { Idle, Wander, Procedural, Eat }

        private const float MinInterval = 1;
        private const float MaxInterval = 3;

        [Export]
        public int MaxSpeed { get; set; } = 40;
        [Export]
        public int Acceleration { get; set; } = 8;
        [Export]
        public int Friction { get; set; } = 4;

        private MainState _mainState = MainState.Idle;
        private int _maxSpeed;
        private int _acceleration;

        private readonly Array<Vector2I> _validTilePositions = new();

        private Timer _stateTimer;

        private WorldLayer _tileMapLayer;
        private AnimationTree _animationTree;
        private AnimationNodeStateMachinePlayback _animationState;
        private NavigationAgent2D _navigationAgent2d;
        private NPCAgent2D _npcAgent2d;

        private EmoteController _emoteController;
        private FloatTextController _floatTextController;

        public override void _Ready()
        {
            if (Engine.IsEditorHint()) return;

            _maxSpeed = MaxSpeed;
            _acceleration = Acceleration;

            _stateTimer = new Timer()
            {
                WaitTime = GD.RandRange(MinInterval, MaxInterval),
                OneShot = true,
                Autostart = true
            };

            _stateTimer.Timeout += RandomizeMainState;
            AddChild(_stateTimer);

            _tileMapLayer = GetNode<WorldLayer>("%WorldLayer");

            _animationTree = GetNode<AnimationTree>("AnimationTree");
            _animationState = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");
            _navigationAgent2d = GetNode<NavigationAgent2D>("NavigationAgent2D");
            _npcAgent2d = GetNode<NPCAgent2D>("NPCAgent2D");

            _emoteController = GetNode<EmoteController>("EmoteController");
            _floatTextController = GetNode<FloatTextController>("FloatTextController");

            _animationTree.AnimationFinished += OnAnimationFinished;
            _navigationAgent2d.VelocityComputed += OnNavigationAgentVelocityComputed;

            _npcAgent2d.ExecutionStarted += OnExecutionStarted;
            _npcAgent2d.ExecutionEnded += OnExecutionEnded;
            _npcAgent2d.ActionStarted += OnActionStarted;
            _npcAgent2d.StateEntered += OnStateEntered;
            _npcAgent2d.StateExited += OnStateExited;
            _npcAgent2d.EventTriggered += OnEventTriggered;

            SetupTilePositions();
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

        private void ShowFloatText(ResourceType resType, string text)
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
            _mainState = CoreHelpers.ShuffleEnum<MainState>().Where((x) =>
                x != MainState.Procedural && x != MainState.Eat
            ).First();

            switch (_mainState)
            {
                case MainState.Idle:
                    _stateTimer.Start(GD.RandRange(MinInterval, MaxInterval));
                    break;
                case MainState.Wander:
                    Vector2 wanderTarget = PickTargetPosition();
                    _navigationAgent2d.TargetPosition = wanderTarget;
                    break;
            }
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

        private void AddActorObstacles(Node2D partner)
        {
            _tileMapLayer.Obstacles.Add(this);
            _tileMapLayer.Obstacles.Add(partner);
            _tileMapLayer.NotifyRuntimeTileDataUpdate();
        }

        private void RemoveActorObstacles(Node2D partner)
        {
            _tileMapLayer.Obstacles.Remove(this);
            _tileMapLayer.Obstacles.Remove(partner);
            _tileMapLayer.NotifyRuntimeTileDataUpdate();
        }

        private void AddSingleObstacle()
        {
            _tileMapLayer.Obstacles.Add(this);
            _tileMapLayer.NotifyRuntimeTileDataUpdate();
        }

        private void RemoveSingleObstacle()
        {
            _tileMapLayer.Obstacles.Remove(this);
            _tileMapLayer.NotifyRuntimeTileDataUpdate();
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

        private void OnActionStarted(Variant action)
        {
            ActionType actionType = action.As<ActionType>();

            if (actionType == ActionType.Theft)
            {
                _emoteController.ShowEmoteBubble(Emote.Hum);
            }
            else if (actionType == ActionType.Eat)
            {
                _mainState = MainState.Eat;
            }
        }

        private void OnExecutionEnded()
        {
            RandomizeMainState();
        }

        private void OnStateEntered(Variant state, Array<Variant> data)
        {
            ActionState actionState = state.As<ActionState>();

            _maxSpeed = actionState == ActionState.Engage ? MaxSpeed + 10 : MaxSpeed;
            _acceleration = actionState == ActionState.Engage ? Acceleration + 2 : Acceleration;

            if (actionState == ActionState.Talk || actionState == ActionState.Petition
                || actionState == ActionState.Interact || actionState == ActionState.Interrogate)
            {
                Node2D partner = data[0].As<Node2D>();
                FacePartner(partner);
            }

            if (actionState == ActionState.Talk || actionState == ActionState.Petition
                || actionState == ActionState.Interrogate)
            {
                Node2D partner = data[0].As<Node2D>();
                AddActorObstacles(partner);
            }
        }

        private void OnStateExited(Variant state, Array<Variant> data)
        {
            ActionState actionState = state.As<ActionState>();

            _maxSpeed = MaxSpeed;
            _acceleration = Acceleration;

            if (actionState == ActionState.Steal)
            {
                float amountStolen = data[1].As<float>();
                _floatTextController.ShowFloatText(ResourceType.Money, amountStolen.ToString(), false);
            }
            else if (actionState == ActionState.Talk || actionState == ActionState.Petition
                || actionState == ActionState.Interact || actionState == ActionState.Interrogate)
            {
                _mainState = MainState.Procedural;
                _emoteController.Deactivate();
            }
            else if (actionState == ActionState.Wander)
            {
                bool durationReached = data[0].As<bool>();

                if (durationReached)
                {
                    _emoteController.ShowEmoteBubble(Emote.Ellipsis);
                }
            }
            else if (actionState == ActionState.Eat)
            {
                int satiationIncrease = data[0].As<int>();
                _floatTextController.ShowFloatText(
                    ResourceType.Satiation,
                    satiationIncrease.ToString()
                );
            }
            else if (actionState == ActionState.Assess)
            {
                bool isIndeterminate = data[0].As<bool>();

                if (isIndeterminate)
                {
                    _emoteController.ShowEmoteBubble(Emote.Dizzy);
                }
            }

            if (actionState == ActionState.Petition || actionState == ActionState.Talk
                || actionState == ActionState.Interrogate)
            {
                Node2D partner = data[0].As<Node2D>();
                RemoveActorObstacles(partner);
            }

            if (actionState == ActionState.Petition)
            {
                bool isAccepted = data[3].As<bool>();

                if (isAccepted)
                {
                    ResourceType type = data[1].As<ResourceType>();
                    int amountPetitioned = data[2].As<int>();
                    _floatTextController.ShowFloatText(type, amountPetitioned.ToString());
                }
                else
                {
                    int companionshipChange = data[4].As<int>();
                    _floatTextController.ShowFloatText(
                        ResourceType.Companionship,
                        companionshipChange.ToString()
                    );
                }
            }
            else if (actionState == ActionState.Talk)
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
        }

        private void OnEventTriggered(Variant eventType)
        {
            EventType type = eventType.As<EventType>();

            if (type == EventType.CrimeWitnessed)
            {
                _emoteController.ShowEmoteBubble(Emote.Interrobang);
            }
            else if (type == EventType.Detained)
            {
                // Immobilize
                _stateTimer.Stop();
            }
            else if (type == EventType.Captured)
            {
                RandomizeMainState();
            }
        }

        private void FacePartner(Node2D partner)
        {
            Vector2 directionToFace = GlobalPosition.DirectionTo(partner.GlobalPosition);

            _animationTree.Set("parameters/Idle/blend_position", directionToFace.X);
            _animationState.Travel("Idle");

            _mainState = MainState.Idle;
            _emoteController.Activate();
        }
    }
}
