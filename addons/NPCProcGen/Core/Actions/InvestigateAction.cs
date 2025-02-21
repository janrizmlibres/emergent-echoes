using System;
using Godot;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    public class InvestigateAction : BaseAction
    {
        private ActorTag2D _target = null;
        private Crime _crime;

        private bool _doneResearching = false;

        private ResearchState _researchState;
        private SearchState _searchState;

        public InvestigateAction(ActorContext context, Crime crime)
            : base(context, ActionType.Investigate)
        {
            _crime = crime;
        }

        protected override void InitializeStates()
        {
            _researchState = new(_actorContext, _stateContext);
        }

        protected override BaseState GetStartingState()
        {
            return _doneResearching ? _searchState : _researchState;
        }

        public void SetupInteractStates()
        {
            _doneResearching = true;

            Tuple<ActorTag2D, Vector2> targetWitnessData = _crime
                .GetRandomWitnessData(_actorContext.GetNPCAgent2D());

            if (targetWitnessData == null)
            {
                _actorContext.Executor.FinishAction();
                return;
            }

            _target = targetWitnessData.Item1;

            _searchState = new(_actorContext, _stateContext, _target);
            _stateContext.WanderState = new(_actorContext, _stateContext, _target);
            _stateContext.ApproachState = new EngageState(_actorContext, _stateContext, _target,
                Waypoint.Lateral);
            _stateContext.WaitState = new(_actorContext, _stateContext, _target);
            _stateContext.ContactState = new InterrogateState(_actorContext, _stateContext, _crime,
                _target);

            TransitionTo(_searchState);
        }
    }
}