using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    public class InteractAction : BaseAction
    {
        private readonly ActorTag2D _target;

        private InteractionState _interactState;

        public InteractAction(ActorContext context, ActorTag2D target)
            : base(context, ActionType.Interact)
        {
            _target = target;
        }

        protected override void InitializeStates()
        {
            _interactState = new InteractionState(_actorContext, _stateContext, _target);
        }

        protected override BaseState GetStartingState() => _interactState;
    }
}