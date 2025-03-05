using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Internal;

// ReSharper disable once CheckNamespace
namespace NPCProcGen.Core.States
{
    public class InteractState(
        ActorContext actorContext,
        StateContext stateContext,
        ActorTag2D target)
        : BaseState(actorContext, stateContext, ActionState.Interact)
    {
        protected override EnterParameters GetEnterData()
        {
            return new EnterParameters
            {
                StateName = "InteractionState",
                Data = [target.GetParent<Node2D>()]
            };
        }

        protected override ExitParameters GetExitData()
        {
            return new ExitParameters
            {
                Data = []
            };
        }

        protected override void ExecuteEnter()
        {
            NotifManager.Instance.NotifyInteractionStarted(ActorContext.Actor);
        }

        protected override void ExecuteExit()
        {
            NotifManager.Instance.NotifyInteractionEnded(ActorContext.Actor);
        }
    }
}