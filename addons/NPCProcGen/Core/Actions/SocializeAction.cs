using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    public class SocializeAction : BaseAction
    {
        private SeekState _seekState;

        public SocializeAction(ActorContext context) : base(context, ActionType.Socialize)
        { }

        protected override void InitializeStates()
        {
            _seekState = new SeekState(_actorContext, _stateContext, SetupInteractStates);
        }

        private void SetupInteractStates(ActorTag2D partner)
        {
            _stateContext.ApproachState = new EngageState(
                _actorContext,
                _stateContext,
                partner,
                Waypoint.Lateral
            );
            _stateContext.WaitState = new(_actorContext, _stateContext, partner);
            _stateContext.ContactState = new TalkState(_actorContext, _stateContext, partner);

            TransitionTo(_stateContext.ApproachState);
        }

        protected override BaseState GetStartingState() => _seekState;
    }
}