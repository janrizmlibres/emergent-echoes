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
            StateContext.ApproachState = new EngageState(ActorContext, StateContext, _target,
                Waypoint.Omni, true);
            StateContext.ContactState = new CaptureState(
                ActorContext,
                StateContext,
                _target,
                _crime
            );
            StateContext.StartingState = StateContext.ApproachState;
        }

        public ActorTag2D GetTargetActor() => _target;
    }
}