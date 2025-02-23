using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    public class InterrogateAction : BaseAction, ITargetedAction
    {
        private readonly ActorTag2D _target;
        private readonly Crime _crime;

        private SearchState _searchState;

        public InterrogateAction(ActorContext context, ActorTag2D target, Crime crime)
            : base(context, ActionType.Interrogate)
        {
            _target = target;
            _crime = crime;
        }

        protected override void InitializeStates()
        {
            _searchState = new SearchState(_actorContext, _stateContext, _target);

            _stateContext.WanderState = new(_actorContext, _stateContext, _target);
            _stateContext.ApproachState = new EngageState(_actorContext, _stateContext, _target,
                Waypoint.Lateral);
            _stateContext.WaitState = new(_actorContext, _stateContext, _target);
            _stateContext.ContactState = new InterrogateState(_actorContext, _stateContext, _target,
                _crime);
        }

        protected override BaseState GetStartingState() => _searchState;
        public ActorTag2D GetTargetActor() => _target;
    }
}