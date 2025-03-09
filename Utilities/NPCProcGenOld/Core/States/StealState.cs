using Godot;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Internal;

// ReSharper disable once CheckNamespace
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
                Data = []
            };
        }

        protected override ExitParameters GetExitData()
        {
            return new ExitParameters
            {
                Data =
                [
                    Variant.From(_targetResource),
                    _amountToSteal
                ]
            };
        }

        protected override void ExecuteEnter()
        {
            ResourceManager.Instance.TranserResources(
                _targetActor,
                ActorContext.Actor,
                _targetResource,
                _amountToSteal
            );

            StateContext.Action.TransitionTo(StateContext.FleeState);
        }

        protected override void ExecuteExit()
        {
            Crime newCrime = new(CrimeCategory.Theft, ActorContext.Actor);
            NotifManager.Instance.NotifyCrimeCommitted(
                ActorContext.Actor,
                _targetActor,
                newCrime
            );
            Sensor.RecordCrime(newCrime);
        }

        private float ComputeStealAmount()
        {
            ResourceManager resMgr = ResourceManager.Instance;
            ResourceStat ownerResource = resMgr.GetResource(_targetResource, ActorContext.Actor);
            ResourceStat targetResource = resMgr.GetResource(_targetResource, _targetActor);
            return CommonUtils.CalculateSkewedAmount(ownerResource, 0.5f, 2, targetResource.Amount);
        }
    }
}