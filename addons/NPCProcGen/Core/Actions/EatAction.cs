using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    public class EatAction : BaseAction
    {
        private EatState _eatState;

        public EatAction(ActorContext context) : base(context, ActionType.Eat) { }

        protected override void InitializeStates()
        {
            _eatState = new EatState(_actorContext, _stateContext);
        }

        protected override BaseState GetStartingState() => _eatState;
    }
}