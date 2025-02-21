using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    public class PursuitAction : BaseAction
    {
        private readonly ActorTag2D _target;

        private ResearchState _researchState;

        public PursuitAction(ActorContext context, ActorTag2D target)
            : base(context, ActionType.Pursuit)
        {
            _target = target;
        }

        protected override void InitializeStates()
        {
            _researchState = new(_actorContext, _stateContext);

            _stateContext.ApproachState = new EngageState(_actorContext, _stateContext, _target,
                Waypoint.Omni);
            _stateContext.ContactState = new CaptureState(_actorContext, _stateContext, _target);
        }

        protected override BaseState GetStartingState() => _researchState;
    }
}