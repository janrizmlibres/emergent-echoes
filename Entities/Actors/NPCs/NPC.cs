using Godot;
using Godot.Collections;
using NPCProcGen;
using NPCProcGen.Core.Components.Enums;
using System.Linq;
using EmergentEchoes.Utilities.Game.Enums;
using EmergentEchoes.Utilities.Game;
using EmergentEchoes.Utilities;

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

        private readonly Array<Vector2I> _validTilePositions = new();

        private Timer _stateTimer;

        private TileMapLayer _tileMapLayer;
        private AnimationTree _animationTree;
        private AnimationNodeStateMachinePlayback _animationState;
        private NavigationAgent2D _navigationAgent2d;
        private NPCAgent2D _npcAgent2d;

        private EmoteController _emoteController;
        private FloatTextController _floatTextController;

        public override void _Ready()
        {
            if (Engine.IsEditorHint()) return;

            _stateTimer = new Timer()
            {
                WaitTime = GD.RandRange(MinInterval, MaxInterval),
                OneShot = true,
                Autostart = true
            };

            _stateTimer.Timeout += RandomizeMainState;
            AddChild(_stateTimer);

            _tileMapLayer = GetNode<TileMapLayer>("%TileMapLayer");

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
            _npcAgent2d.InteractionStarted += OnInteractionStarted;
            _npcAgent2d.InteractionEnded += OnInteractionEnded;
            _npcAgent2d.ActionStateEntered += OnActionStateEntered;
            _npcAgent2d.ActionStateExited += OnActionStateExited;

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
            _navigationAgent2d.Velocity = Velocity.MoveToward(direction * MaxSpeed, Acceleration);

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

        private void OnExecutionStarted(Variant action)
        {
            ActionType actionType = action.As<ActionType>();

            _mainState = MainState.Procedural;
            _stateTimer.Stop();

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

        private void OnInteractionStarted(Variant state, Array<Variant> data)
        {
            _stateTimer.Stop();

            Node2D partner = data[0].As<Node2D>();
            FacePartner(partner);
        }

        private void OnInteractionEnded()
        {
            _mainState = MainState.Procedural;
            _emoteController.Deactivate();
        }

        private void OnActionStateEntered(Variant state, Array<Variant> data)
        {
            ActionState actionState = state.As<ActionState>();

            if (actionState == ActionState.Talk || actionState == ActionState.Petition)
            {
                Node2D partner = data[0].As<Node2D>();
                FacePartner(partner);
            }
        }

        private void OnActionStateExited(Variant state, Array<Variant> data)
        {
            ActionState actionState = state.As<ActionState>();

            if (actionState == ActionState.Steal)
            {
                float amountStolen = data[1].As<float>();
                _floatTextController.ShowFloatText(ResourceType.Money, amountStolen.ToString(), false);
            }
            else if (actionState == ActionState.Talk || actionState == ActionState.Petition)
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

            if (actionState == ActionState.Petition)
            {
                bool isAccepted = data[0].As<bool>();

                if (isAccepted)
                {
                    ResourceType type = data[2].As<ResourceType>();
                    int amountPetitioned = data[3].As<int>();
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

                if (partner is not NPC) return;

                float companionshipIncrease = data[1].As<float>();
                NPC npc = partner as NPC;

                npc.ShowFloatText(ResourceType.Companionship, companionshipIncrease.ToString());
                _floatTextController.ShowFloatText(
                    ResourceType.Companionship,
                    companionshipIncrease.ToString()
                );
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
