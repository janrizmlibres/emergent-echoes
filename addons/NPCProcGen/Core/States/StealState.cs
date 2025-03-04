using Godot;
using Godot.Collections;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.States
{
    public class StealState : BaseState
    {
        private readonly ActorTag2D _targetActor;
        private readonly ResourceType _targetResource;
        private readonly float _amountToSteal;

        public StealState(ActorContext actorContext, StateContext stateContext, ActorTag2D target,
            ResourceType type) : base(actorContext, stateContext, ActionState.Steal)
        {
            _targetActor = target;
            _targetResource = type;
            _amountToSteal = ComputeStealAmount();
        }

        protected override EnterParameters GetEnterData()
        {
            return new EnterParameters
            {
                StateName = "StealState",
                Data = new Array<Variant>()
            };
        }

        protected override ExitParameters GetExitData()
        {
            return new ExitParameters
            {
                Data = new Array<Variant>()
                {
                    Variant.From(_targetResource),
                    _amountToSteal
                }
            };
        }

        protected override void ExecuteEnter()
        {
            ResourceManager.Instance.TranserResources(
                _targetActor,
                _actorContext.Actor,
                _targetResource,
                _amountToSteal
            );

            _stateContext.Action.TransitionTo(_stateContext.FleeState);
        }

        protected override void ExecuteExit()
        {
            Crime newCrime = new(CrimeCategory.Theft, _actorContext.Actor);
            NotifManager.Instance.NotifyCrimeCommitted(
                _actorContext.Actor,
                _targetActor,
                newCrime
            );
            Sensor.RecordCrime(newCrime);
        }

        private float ComputeStealAmount()
        {
            ResourceManager resMgr = ResourceManager.Instance;
            ResourceStat ownerResource = resMgr.GetResource(_targetResource, _actorContext.Actor);
            ResourceStat targetResource = resMgr.GetResource(_targetResource, _targetActor);
            return CommonUtils.CalculateSkewedAmount(ownerResource, 0.5f, 2, targetResource.Amount);
        }
    }
}