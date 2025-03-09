using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.States
{
    public class FleeState : BaseState, INavigationState
    {
        private const float MinDistance = 200;
        private const float MaxDistance = 400;

        private Vector2 _fleePosition;

        public FleeState(ActorContext actorContext, StateContext stateContext)
            : base(actorContext, stateContext, ActionState.Flee) { }

        protected override void ExecuteEnter()
        {
            _fleePosition = CommonUtils.GetRandomPosInCircularArea(
                ActorContext.ActorNode2D.GlobalPosition,
                MaxDistance,
                MinDistance
            );
        }

        protected override EnterParameters GetEnterData()
        {
            return new EnterParameters
            {
                StateName = "FleeState",
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

        public bool IsNavigating()
        {
            return true;
        }

        public Vector2 GetTargetPosition()
        {
            return _fleePosition;
        }

        public bool OnNavigationComplete()
        {
            ActorContext.Executor.FinishAction();
            return true;
        }
    }
}