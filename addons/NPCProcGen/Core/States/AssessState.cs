using Godot;
using Godot.Collections;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.States
{
    public class AssessState : BaseState
    {
        private const int MinDuration = 20;
        private const int MaxDuration = 30;

        private readonly Crime _crime;
        private readonly bool _caseClosed;

        private float _duration;

        public AssessState(ActorContext actorContext, StateContext stateContext,
            Crime crime, bool caseClosed)
            : base(actorContext, stateContext, ActionState.Assess)
        {
            _crime = crime;
            _caseClosed = caseClosed;
            _duration = GD.RandRange(MinDuration, MaxDuration);
        }

        protected override EnterParameters GetEnterData()
        {
            return new EnterParameters
            {
                StateName = "AssessState",
                Data = new Array<Variant>()
            };
        }

        protected override ExitParameters GetExitData()
        {
            return new ExitParameters
            {
                Data = new Array<Variant> { CommonUtils.DutyIncrease }
            };
        }

        protected override void ExecuteExit()
        {
            _crime.AssessmentDone = true;

            if (_caseClosed)
            {
                _crime.Status = CrimeStatus.Unsolved;
            }

            ResourceManager.Instance.ModifyResource(
                ResourceType.Duty,
                CommonUtils.DutyIncrease,
                ActorContext.Actor
            );
        }

        public override void Update(double delta)
        {
            _duration -= (float)delta;

            if (_duration <= 0)
            {
                ActorContext.Executor.FinishAction();
            }
        }
    }
}