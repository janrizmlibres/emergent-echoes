using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    public class CloseCaseAction : BaseAction
    {
        private ResearchState _researchState;

        public CloseCaseAction(ActorContext context) : base(context, ActionType.CloseCase)
        { }

        protected override void InitializeStates()
        {
            _researchState = new(_actorContext, _stateContext);
        }

        protected override BaseState GetStartingState() => _researchState;
    }
}