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
            StateContext.StartingState = new SeekState(
                ActorContext,
                StateContext,
                SetupInteractStates
            );
        }

        private void SetupInteractStates(ActorTag2D target)
        {
            _target = target;

            StateContext.ApproachState = new EngageState(
                ActorContext,
                StateContext,
                _target,
                Waypoint.Lateral
            );
            StateContext.WaitState = new(ActorContext, StateContext, _target);
            StateContext.ContactState = new TalkState(ActorContext, StateContext, _target);

            StateContext.ApproachTarget(_target);
        }

        public ActorTag2D GetTargetActor() => _target;
    }
}