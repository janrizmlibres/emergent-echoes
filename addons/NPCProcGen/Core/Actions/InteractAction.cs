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
            _stateContext.StartingState = new InteractState(_actorContext, _stateContext, _target);
        }

        public ActorTag2D GetTargetActor() => _target;
    }
}