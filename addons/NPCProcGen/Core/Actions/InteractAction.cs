using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    public class InteractAction : BaseAction, ITargetedAction
    {
        private readonly ActorTag2D _target;

        public InteractAction(ActorContext context, ActorTag2D target)
            : base(context, ActionType.Interact)
        {
            _target = target;
        }

        protected override void InitializeStates()
        {
            StateContext.StartingState = new InteractState(ActorContext, StateContext, _target);
        }

        public ActorTag2D GetTargetActor() => _target;
    }
}