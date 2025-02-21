using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    public class TheftAction : BaseAction
    {
        private readonly ActorTag2D _targetActor;
        private readonly ResourceType _targetResource;

        private SearchState _searchState;

        public TheftAction(ActorContext context, ActorTag2D target, ResourceType type)
            : base(context, ActionType.Theft)
        {
            _targetActor = target;
            _targetResource = type;
        }

        protected override void InitializeStates()
        {
            _searchState = new SearchState(
                _actorContext,
                _stateContext,
                _targetActor
            );

            _stateContext.WanderState = new(_actorContext, _stateContext, _targetActor);
            _stateContext.ApproachState = new SneakState(_actorContext, _stateContext, _targetActor);
            _stateContext.ContactState = new StealState(
                _actorContext,
                _stateContext,
                _targetActor,
                _targetResource
            );
            _stateContext.FleeState = new(_actorContext, _stateContext);
        }

        protected override BaseState GetStartingState() => _searchState;
    }
}