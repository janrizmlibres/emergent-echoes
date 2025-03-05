using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.States
{
    public class WanderState : BaseState, INavigationState, IActorDetectionState
    {
        private const float WanderRadius = 100;
        private const int MinInterval = 5;
        private const int MaxInterval = 10;
        private const int MaxDuration = 30;

        private readonly ActorTag2D _target;
        private Vector2 _wanderPosition;
        private Vector2 _origin;

        private bool _isWandering = false;
        private bool _durationReached = false;

        private float _durationTimer = MaxDuration;
        private float _idleTimer;

        public WanderState(ActorContext actorContext, StateContext stateContext, ActorTag2D target)
            : base(actorContext, stateContext, ActionState.Wander)
        {
            _target = target;
            _wanderPosition = actorContext.ActorNode2D.GlobalPosition;
            _idleTimer = GD.RandRange(MinInterval, MaxInterval);
        }

        protected override void ExecuteEnter()
        {
            _origin = ActorContext.ActorNode2D.GlobalPosition;
        }

        protected override EnterParameters GetEnterData()
        {
            return new EnterParameters
            {
                StateName = "WanderState",
                Data = new Array<Variant>()
            };
        }

        protected override ExitParameters GetExitData()
        {
            return new ExitParameters
            {
                Data = new Array<Variant>() { _durationReached }
            };
        }

        public override void Update(double delta)
        {
            _durationTimer -= (float)delta;

            if (_durationTimer <= 0)
            {
                _durationReached = true;
                ActorContext.Executor.FinishAction();
                return;
            }

            if (_isWandering) return;

            _idleTimer -= (float)delta;

            if (_idleTimer <= 0)
            {
                _wanderPosition = CommonUtils.GetRandomPosInCircularArea(_origin, WanderRadius);
                _idleTimer = GD.RandRange(MinInterval, MaxInterval);
                _isWandering = true;
            }
        }

        public bool IsNavigating()
        {
            return _isWandering;
        }

        public Vector2 GetTargetPosition()
        {
            return _wanderPosition;
        }

        public bool OnNavigationComplete()
        {
            _isWandering = false;
            return true;
        }

        public void OnActorDetected(ActorTag2D target)
        {
            if (target == _target)
            {
                StateContext.ApproachTarget(_target);
            }
        }
    }
}