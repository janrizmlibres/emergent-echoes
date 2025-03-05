using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    public class PursuitAction : BaseAction, ITargetedAction
    {
        private readonly ActorTag2D _target;
        private readonly Crime _crime;

        public PursuitAction(ActorContext context, ActorTag2D target, Crime crime)
            : base(context, ActionType.Pursuit)
        {
            _target = target;
            _crime = crime;
        }

        protected override void InitializeStates()
        {
            _stateContext.ApproachState = new EngageState(_actorContext, _stateContext, _target,
                Waypoint.Omni, true);
            _stateContext.ContactState = new CaptureState(
                _actorContext,
                _stateContext,
                _target,
                _crime
            );
            _stateContext.StartingState = _stateContext.ApproachState;
        }

        public ActorTag2D GetTargetActor() => _target;
    }
}