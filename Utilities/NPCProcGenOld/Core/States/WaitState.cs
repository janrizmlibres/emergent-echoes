using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.States
{
    public class WaitState : BaseState, INavigationState
    {
        private const float WaitDistance = 40;

        private readonly ActorTag2D _target;

        public WaitState(ActorContext actorContext, StateContext stateContext, ActorTag2D target)
            : base(actorContext, stateContext, ActionState.Wait)
        {
            _target = target;
        }

        protected override void Subscribe()
        {
            NotifManager.Instance.InteractionEnded += OnTargetInteractionEnded;
        }

        protected override EnterParameters GetEnterData()
        {
            return new EnterParameters
            {
                StateName = "WaitState",
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

        public override void Unsubscribe()
        {
            NotifManager.Instance.InteractionEnded -= OnTargetInteractionEnded;
        }

        public bool IsNavigating()
        {
            return true;
        }

        public Vector2 GetTargetPosition()
        {
            Vector2 directionToInitiator = _target.GetParent<Node2D>().GlobalPosition.DirectionTo(
                ActorContext.ActorNode2D.GlobalPosition
            );
            return _target.GetParent<Node2D>().GlobalPosition + directionToInitiator * WaitDistance;
        }

        public bool OnNavigationComplete()
        {
            return true;
        }

        private void OnTargetInteractionEnded(ActorTag2D target)
        {
            if (target != _target) return;
            StateContext.Action.TransitionTo(StateContext.ApproachState);
        }
    }
}