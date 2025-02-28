using Godot;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    public class PetitionAction : BaseAction, ITargetedAction
    {
        private readonly ActorTag2D _targetActor;
        private readonly ResourceType _targetResource;

        public PetitionAction(ActorContext context, ActorTag2D target, ResourceType type)
            : base(context, ActionType.Petition)
        {
            _targetActor = target;
            _targetResource = type;
        }

        protected override void InitializeStates()
        {
            _stateContext.StartingState = new SearchState(_actorContext, _stateContext, _targetActor);
            _stateContext.WanderState = new(_actorContext, _stateContext, _targetActor);
            _stateContext.ApproachState = new EngageState(_actorContext, _stateContext, _targetActor,
                Waypoint.Lateral);
            _stateContext.WaitState = new(_actorContext, _stateContext, _targetActor);
            _stateContext.ContactState = new PetitionState(_actorContext, _stateContext, _targetActor,
                _targetResource);
        }

        protected override void ExecuteRun()
        {
            _actorContext.Sensor.SetPetitionResourceType(_targetResource);
        }

        public override void Terminate()
        {
            _actorContext.Sensor.ClearPetitionResourceType();
        }

        public ActorTag2D GetTargetActor() => _targetActor;
    }
}