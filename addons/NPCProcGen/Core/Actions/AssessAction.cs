using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    public class AssessAction : BaseAction
    {
        private AssessState _reviewState;

        public AssessAction(ActorContext context) : base(context, ActionType.Assess) { }

        protected override void InitializeStates()
        {
            _reviewState = new AssessState(_actorContext, _stateContext);
        }

        protected override BaseState GetStartingState() => _reviewState;
    }
}