using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.States
{
    public class InteractionState : BaseState
    {
        private readonly ActorTag2D _target;

        public InteractionState(ActorContext actorContext, StateContext stateContext,
            ActorTag2D target)
            : base(actorContext, stateContext, ActionState.Interact)
        {
            _target = target;
        }

        protected override EnterParameters GetEnterParameters()
        {
            return new EnterParameters
            {
                StateName = "InteractionState",
                Data = new Array<Variant> { _target.GetParent<Node2D>() }
            };
        }

        protected override ExitParameters GetExitParameters()
        {
            return new ExitParameters
            {
                Data = new Array<Variant>()
            };
        }

        protected override void ExecuteEnterLogic()
        {
            NotifManager.Instance.NotifyInteractionStarted(_actorContext.Actor);
        }

        protected override void ExecuteExitLogic()
        {
            NotifManager.Instance.NotifyInteractionEnded(_actorContext.Actor);
        }
    }
}