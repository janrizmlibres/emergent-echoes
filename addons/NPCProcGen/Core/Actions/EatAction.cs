using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    public class EatAction : BaseAction
    {
        public EatAction(ActorContext context) : base(context, ActionType.Eat) { }

        protected override void InitializeStates()
        {
            _stateContext.StartingState = new EatState(_actorContext, _stateContext);
        }
    }
}