using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    public class TheftAction : BaseAction, ITargetedAction
    {
        private readonly ActorTag2D _targetActor;
        private readonly ResourceType _targetResource;

        public TheftAction(ActorContext context, ActorTag2D target, ResourceType type)
            : base(context, ActionType.Theft)
        {
            _targetActor = target;
            _targetResource = type;
        }

        protected override void InitializeStates()
        {
            StateContext.StartingState = new SearchState(
                ActorContext,
                StateContext,
                _targetActor
            );
            StateContext.WanderState = new(ActorContext, StateContext, _targetActor);
            StateContext.ApproachState = new SneakState(ActorContext, StateContext, _targetActor);
            StateContext.ContactState = new StealState(
                ActorContext,
                StateContext,
                _targetActor,
                _targetResource
            );
            StateContext.FleeState = new(ActorContext, StateContext);
        }

        public ActorTag2D GetTargetActor() => _targetActor;
    }
}