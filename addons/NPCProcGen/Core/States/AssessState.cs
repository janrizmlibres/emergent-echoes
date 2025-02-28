using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.States
{
    public class AssessState : BaseState
    {
        private const int MinDuration = 20;
        private const int MaxDuration = 30;

        private float _duration;

        public AssessState(ActorContext actorContext, StateContext stateContext)
            : base(actorContext, stateContext, ActionState.Assess)
        {
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
                Data = new Array<Variant>()
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
            _actorContext.Executor.FinishAction();
        }
    }
}