using System;
using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.States
{
    public class MoveState : BaseState, INavigationState
    {
        private Vector2 _targetPosition;

        public MoveState(ActorContext actorContext, StateContext stateContext,
            Vector2 targetPosition)
            : base(actorContext, stateContext, ActionState.Move)
        {
            _targetPosition = targetPosition;
        }

        protected override EnterParameters GetEnterData()
        {
            return new EnterParameters
            {
                StateName = "MoveState",
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
            return _targetPosition;
        }

        public bool OnNavigationComplete()
        {
            _actorContext.Executor.FinishAction();
            return true;
        }
    }
}