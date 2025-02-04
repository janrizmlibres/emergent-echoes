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
        private enum MainState { Idle, Wander, Procedural }

        private const float MinInterval = 1;
        private const float MaxInterval = 3;
        private const float MinBubbleInterval = 2;
        private const float MaxBubbleInterval = 5;

        [Export]
        public int MaxSpeed { get; set; } = 40;
        [Export]
        public int Acceleration { get; set; } = 8;
        [Export]
        public int Friction { get; set; } = 4;

        private MainState _mainState = MainState.Idle;

        private readonly Array<Vector2I> _validTilePositions = new();

        private Timer _mainStateTimer;
        private Timer _talkBubbleTimer;

        private TileMapLayer _tileMapLayer;
        private AnimationTree _animationTree;
        private AnimationNodeStateMachinePlayback _animationState;
        private NavigationAgent2D _navigationAgent2d;
        private NPCAgent2D _npcAgent2d;

        public override void _Ready()
        {
            if (Engine.IsEditorHint()) return;

            _mainStateTimer = new Timer()
            {
                WaitTime = GD.RandRange(MinInterval, MaxInterval),
                OneShot = true,
                Autostart = true
            };

            _talkBubbleTimer = new Timer()
            {
                OneShot = true,
            };

            _mainStateTimer.Timeout += RandomizeMainState;
            _talkBubbleTimer.Timeout += ShowNextBubble;
            AddChild(_mainStateTimer);
            AddChild(_talkBubbleTimer);

            _tileMapLayer = GetNode<TileMapLayer>("%TileMapLayer");

            _animationTree = GetNode<AnimationTree>("AnimationTree");
            _animationState = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");
            _navigationAgent2d = GetNode<NavigationAgent2D>("NavigationAgent2D");
            _npcAgent2d = GetNode<NPCAgent2D>("NPCAgent2D");

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
            }

            MoveAndSlide();
        }

        private void HandleWanderState()
        {
            if (_navigationAgent2d.IsNavigationFinished())
            {
                GD.Print($"{Name} is done wandering. Randomizing main state.");
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
            _mainState = CoreHelpers.ShuffleEnum<MainState>().Where(x => x != MainState.Procedural).First();

            switch (_mainState)
            {
                case MainState.Idle:
                    GD.Print($"{Name} is idling.");
                    _mainStateTimer.Start(GD.RandRange(MinInterval, MaxInterval));
                    break;
                case MainState.Wander:
                    GD.Print($"{Name} is wandering.");
                    Vector2 wanderTarget = PickTargetPosition();
                    _navigationAgent2d.TargetPosition = wanderTarget;
                    break;
            }
        }

        private void ShowNextBubble()
        {
            Emote emoteValue = CoreHelpers.ShuffleEnum<Emote>().First();
            EmoteManager.ShowEmoteBubble(this, emoteValue);
            _talkBubbleTimer.Start(GD.RandRange(MinBubbleInterval, MaxBubbleInterval));
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

        private void OnNavigationAgentVelocityComputed(Vector2 safeVelocity)
        {
            Velocity = safeVelocity;
        }

        private void OnExecutionStarted(Variant action)
        {
            ActionType actionType = action.As<ActionType>();

            _mainState = MainState.Procedural;
            _mainStateTimer.Stop();

            if (actionType == ActionType.Theft)
            {
                EmoteManager.ShowEmoteBubble(this, Emote.Hum);
            }
            else if (actionType == ActionType.Eat)
            {
                EmoteManager.ShowEmoteBubble(this, Emote.Sweat);
            }
        }

        private void OnExecutionEnded()
        {
            GD.Print($"{Name} is done executing. Randomizing main state.");
            RandomizeMainState();
        }

        private void OnInteractionStarted(Variant state, Array<Variant> data)
        {
            _mainStateTimer.Stop();
            FacePartner(data[0].As<Node2D>());
        }

        private void OnInteractionEnded()
        {
            if (_npcAgent2d.IsActive())
            {
                _mainState = MainState.Procedural;
            }
            else
            {
                RandomizeMainState();
            }

            _talkBubbleTimer.Stop();
        }

        private void OnActionStateEntered(Variant state, Array<Variant> data)
        {
            ActionState actionState = state.As<ActionState>();

            if (actionState == ActionState.Talk || actionState == ActionState.Petition)
            {
                FacePartner(data[0].As<Node2D>());
            }
        }

        private void OnActionStateExited(Variant state, Array<Variant> data)
        {
            ActionState actionState = state.As<ActionState>();

            if (actionState == ActionState.Steal)
            {
                float amountStolen = data[1].As<float>();
                FloatTextManager.ShowFloatText(this, amountStolen.ToString());
            }
            else if (actionState == ActionState.Talk || actionState == ActionState.Petition)
            {
                GD.Print($"{Name} is done interacting.");
                _mainState = MainState.Procedural;
                _talkBubbleTimer.Stop();
            }
            else if (actionState == ActionState.Wander)
            {
                bool durationReached = data[0].As<bool>();

                if (durationReached)
                {
                    EmoteManager.ShowEmoteBubble(this, Emote.Ellipsis);
                }
            }
        }

        private void FacePartner(Node2D partner)
        {
            Vector2 directionToFace = GlobalPosition.DirectionTo(partner.GlobalPosition);

            _animationTree.Set("parameters/Idle/blend_position", directionToFace.X);
            _animationState.Travel("Idle");

            _mainState = MainState.Idle;
            _talkBubbleTimer.Start(GD.RandRange(MinBubbleInterval, MaxBubbleInterval));
            GD.Print($"{Name} is interacting with {partner.Name}");
        }
    }
}
