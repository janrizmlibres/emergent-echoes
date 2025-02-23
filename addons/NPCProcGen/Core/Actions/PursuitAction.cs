using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    public class PursuitAction : BaseAction, ITargetedAction
    {
        private readonly ActorTag2D _target;

        public PursuitAction(ActorContext context) : base(context, ActionType.Pursuit) { }

        protected override void InitializeStates()
        {
            _stateContext.ApproachState = new EngageState(_actorContext, _stateContext, _target,
                Waypoint.Omni);
            _stateContext.WaitState = new(_actorContext, _stateContext, _target);
            _stateContext.ContactState = new CaptureState(_actorContext, _stateContext, _target);
        }

        protected override BaseState GetStartingState() => _stateContext.ApproachState;
        public ActorTag2D GetTargetActor() => _target;
    }
}