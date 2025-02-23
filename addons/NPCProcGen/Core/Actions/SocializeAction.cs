using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    public class SocializeAction : BaseAction, ITargetedAction
    {
        private ActorTag2D _target;

        public SocializeAction(ActorContext context) : base(context, ActionType.Socialize) { }

        protected override void InitializeStates()
        {
            _stateContext.StartingState = new SeekState(
                _actorContext,
                _stateContext,
                SetupInteractStates
            );
        }

        private void SetupInteractStates(ActorTag2D target)
        {
            _target = target;

            _stateContext.ApproachState = new EngageState(
                _actorContext,
                _stateContext,
                _target,
                Waypoint.Lateral
            );
            _stateContext.WaitState = new(_actorContext, _stateContext, _target);
            _stateContext.ContactState = new TalkState(_actorContext, _stateContext, _target);

            TransitionTo(_stateContext.ApproachState);
        }

        public ActorTag2D GetTargetActor() => _target;
    }
}