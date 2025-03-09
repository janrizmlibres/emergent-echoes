using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;
using NPCProcGen.Core.States;

// ReSharper disable once CheckNamespace
namespace NPCProcGen.Core.Actions
{
    public class PetitionAction(ActorContext context, ActorTag2D target, ResourceType type)
        : BaseAction(context, ActionType.Petition), ITargetedAction
    {
        protected override void InitializeStates()
        {
            StateContext.StartingState = new SearchState(ActorContext, StateContext, target);
            StateContext.WanderState = new WanderState(ActorContext, StateContext, target);
            StateContext.ApproachState = new EngageState(ActorContext, StateContext, target,
                Waypoint.Lateral);
            StateContext.WaitState = new WaitState(ActorContext, StateContext, target);
            StateContext.ContactState = new PetitionState(ActorContext, StateContext, target,
                type);
        }

        protected override void ExecuteRun()
        {
            ActorContext.Sensor.SetPetitionResourceType(type);
        }

        protected override void Terminate()
        {
            ActorContext.Sensor.ClearPetitionResourceType();
        }

        public ActorTag2D GetTargetActor() => target;
    }
}