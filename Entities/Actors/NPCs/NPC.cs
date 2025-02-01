using Godot;
using System;
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
        private enum ProceduralState { Moving, Dormant }

        private const float MinInterval = 1;
        private const float MaxInterval = 3;
        private const float MinBubbleInterval = 2;
        private const float MaxBubbleInterval = 5;

        [Export]
        public int MaxSpeed { get; set; } = 60;
        [Export]
        public int Acceleration { get; set; } = 4;
        [Export]
        public int Friction { get; set; } = 4;

        private MainState _mainState = MainState.Idle;
        private ProceduralState _proceduralState = ProceduralState.Moving;

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
            switch (_proceduralState)
            {
                case ProceduralState.Moving:
                    MoveNavigationAgent();
                    break;
                case ProceduralState.Dormant:
                    StopMoving();
                    break;
            }
        }

        private void MoveNavigationAgent()
        {
            _navigationAgent2d.TargetPosition = _npcAgent2d.TargetPosition;

            if (_navigationAgent2d.IsNavigationFinished())
            {
                _npcAgent2d.CompleteNavigation();
                return;
            }

            MoveCharacter();
        }

        private void StopMoving()
        {
            _navigationAgent2d.Velocity = Velocity.MoveToward(Vector2.Zero, Friction);
        }

        private void MoveCharacter()
        {
            Vector2 destination = _navigationAgent2d.GetNextPathPosition();
            Vector2 direction = GlobalPosition.DirectionTo(destination);
            _navigationAgent2d.Velocity = Velocity.MoveToward(direction * MaxSpeed, Acceleration);

            HandleAnimation();
            MoveAndSlide();
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

        private void RandomizeMainState()
        {
            _mainState = CoreHelpers.ShuffleEnum<MainState>().Where(x => x != MainState.Procedural).First();
            _proceduralState = ProceduralState.Moving;

            switch (_mainState)
            {
                case MainState.Idle:
                    _mainStateTimer.Start(GD.RandRange(MinInterval, MaxInterval));
                    break;
                case MainState.Wander:
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

        private void OnExecutionEnded(Variant action)
        {
            RandomizeMainState();
        }

        private void OnActionStateEntered(Variant state, Array<Variant> data)
        {
            ActionState actionState = state.As<ActionState>();

            if ((actionState == ActionState.Talk || actionState == ActionState.Petition)
                && _proceduralState != ProceduralState.Dormant)
            {
                Node2D partner = data[0].As<Node2D>();
                Vector2 directionToFace = GlobalPosition.DirectionTo(partner.GlobalPosition);

                _animationTree.Set("parameters/Idle/blend_position", directionToFace.X);
                _animationState.Travel("Idle");

                _proceduralState = ProceduralState.Dormant;
                _talkBubbleTimer.Start(GD.RandRange(MinBubbleInterval, MaxBubbleInterval));
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
                _proceduralState = ProceduralState.Moving;
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
    }
}