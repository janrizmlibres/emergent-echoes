using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.States
{
    public class SneakState : BaseState, INavigationState
    {
        private readonly ActorTag2D _target;

        public SneakState(ActorContext actorContext, StateContext stateContext, ActorTag2D target)
            : base(actorContext, stateContext, ActionState.Sneak)
        {
            _target = target;
        }

        protected override EnterParameters GetEnterData()
        {
            return new EnterParameters
            {
                StateName = "SneakState",
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
            return _target.GetRearPosition();
        }

        public bool OnNavigationComplete()
        {
            _stateContext.Action.TransitionTo(_stateContext.ContactState);
            return true;
        }
    }
}