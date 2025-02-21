using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.States
{
    public class SearchState : BaseState, INavigationState, IActorDetectionState
    {
        private readonly ActorTag2D _target;
        private Vector2 _lastKnownPosition;

        public SearchState(ActorContext context, StateContext stateContext, ActorTag2D target)
            : base(context, stateContext, ActionState.Search)
        {
            _target = target;
        }

        protected override bool Validate()
        {
            if (_actorContext.GetNPCAgent2D().IsActorInRange(_target))
            {
                _stateContext.ApproachTarget(_target);
                return false;
            }

            Vector2? lastPosition = _actorContext.Memorizer.GetLastKnownPosition(_target);

            if (lastPosition == null)
            {
                _actorContext.Executor.FinishAction();
                return false;
            }

            _lastKnownPosition = lastPosition.Value;
            return true;
        }

        protected override EnterParameters GetEnterParameters()
        {
            return new EnterParameters
            {
                StateName = "SearchState",
                Data = new Array<Variant>()
            };
        }

        protected override ExitParameters GetExitParameters()
        {
            return new ExitParameters
            {
                Data = new Array<Variant>()
            };
        }

        public bool IsNavigating()
        {
            return true;
        }

        public Vector2 GetTargetPosition()
        {
            return _lastKnownPosition;
        }

        public bool OnNavigationComplete()
        {
            _stateContext.Action.TransitionTo(_stateContext.WanderState);
            return true;
        }

        public void OnActorDetected(ActorTag2D target)
        {
            if (target == _target)
            {
                _stateContext.ApproachTarget(_target);
            }
        }
    }
}