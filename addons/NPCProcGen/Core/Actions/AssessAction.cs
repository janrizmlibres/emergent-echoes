using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    public class AssessAction : BaseAction
    {
        public AssessAction(ActorContext context) : base(context, ActionType.Assess) { }

        protected override void InitializeStates()
        {
            _stateContext.StartingState = new AssessState(_actorContext, _stateContext);
        }
    }
}