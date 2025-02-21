using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.States
{
    public class MoveState : BaseState, INavigationState
    {
        private Vector2 _movePosition;

        public MoveState(ActorContext actorContext, StateContext stateContext, Vector2 movePosition)
            : base(actorContext, stateContext, ActionState.Move)
        {
            _movePosition = movePosition;
        }

        protected override EnterParameters GetEnterParameters()
        {
            return new EnterParameters
            {
                StateName = "MoveState",
                Data = new Array<Variant>()
            };
        }

        protected override ExitParameters GetExitParameters()
        {
            return new ExitParameters
            {
                Data = new Array<Variant>()
            };
        }

        /// <summary>
        /// Determines whether the agent is currently navigating.
        /// </summary>
        /// <returns>True if the agent is navigating; otherwise, false.</returns>
        public bool IsNavigating()
        {
            return true;
        }

        /// <summary>
        /// Gets the target position for navigation.
        /// </summary>
        /// <returns>The global position of the target.</returns>
        public Vector2 GetTargetPosition()
        {
            return _movePosition;
        }

        public bool OnNavigationComplete()
        {
            _actorContext.Executor.FinishAction();
            return true;
        }
    }
}