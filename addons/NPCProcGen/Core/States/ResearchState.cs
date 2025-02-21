using System;
using System.Linq;
using Godot;
using Godot.Collections;
using NPCProcGen.Core.Actions;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;
using NPCProcGen.Core.Traits;

namespace NPCProcGen.Core.States
{
    public class ResearchState : BaseState
    {
        private const int MinDuration = 5;
        private const int MaxDuration = 10;

        private float _duration;

        public ResearchState(ActorContext actorContext, StateContext stateContext)
            : base(actorContext, stateContext, ActionState.Research)
        {
            _duration = GD.RandRange(MinDuration, MaxDuration);
        }

        protected override EnterParameters GetEnterParameters()
        {
            return new EnterParameters
            {
                StateName = "ResearchState",
                Data = new Array<Variant>()
            };
        }

        protected override ExitParameters GetExitParameters()
        {
            return new ExitParameters
            {
                Data = new Array<Variant>() { _stateContext.Action.GetType().Name }
            };
        }

        public override void Update(double delta)
        {
            _duration -= (float)delta;

            if (_duration <= 0)
            {
                CompleteState();
            }
        }

        private void CompleteState()
        {
            if (_stateContext.Action is InvestigateAction investigateAction)
            {
                investigateAction?.SetupInteractStates();
            }
            else if (_stateContext.Action is PursuitAction)
            {
                _stateContext.Action.TransitionTo(_stateContext.ContactState);
            }
            else if (_stateContext.Action is CloseCaseAction)
            {
                _actorContext.GetNPCAgent2D().Traits
                    .OfType<LawfulTrait>()
                    .FirstOrDefault()?
                    .MarkCrimeAsUnsolved();

                _actorContext.Executor.FinishAction();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}