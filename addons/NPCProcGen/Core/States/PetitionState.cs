using System;
using Godot;
using Godot.Collections;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Internal;

// ReSharper disable once CheckNamespace
namespace NPCProcGen.Core.States
{
    public class PetitionState : BaseState
    {
        private const float NegotiationDuration = 15;
        private const int CompanionshipIncrease = 3;
        private const int CompanionshipDecrease = -1;

        private readonly ActorTag2D _target;
        private readonly ResourceType _resourceType;
        private readonly int _amount;

        private bool _isAccepted;

        private float _negotiationTimer = NegotiationDuration;

        public PetitionState(ActorContext actorContext, StateContext stateContext,
            ActorTag2D target, ResourceType type)
            : base(actorContext, stateContext, ActionState.Petition)
        {
            _target = target;
            _resourceType = type;
            _amount = CalculateAmount();
        }

        protected override void Subscribe()
        {
            if (_target.IsPlayer()) NotifManager.Instance.PetitionAnswered += OnPetitionAnswered;
        }

        public override void Unsubscribe()
        {
            if (_target.IsPlayer()) NotifManager.Instance.PetitionAnswered -= OnPetitionAnswered;
        }

        protected override EnterParameters GetEnterData()
        {
            return new EnterParameters
            {
                StateName = "PetitionState",
                Data = []
            };
        }

        protected override ExitParameters GetExitData()
        {
            ExitParameters exitData = new()
            {
                Data = [_isAccepted]
            };

            if (_isAccepted)
            {
                exitData.Data.Add(Variant.From(_resourceType));
                exitData.Data.Add(_amount);
            }
            else
            {
                exitData.Data.Add(CompanionshipDecrease);
            }

            return exitData;
        }

        protected override void ExecuteEnter()
        {
            Array<Variant> data =
            [
                _target.GetParent<Node2D>(),
                Variant.From(_resourceType),
                _amount
            ];

            ActorContext.EmitSignal(
                ActorTag2D.SignalName.InteractionStarted,
                Variant.From((InteractionState)_actionState),
                data
            );

            data[0] = ActorContext.ActorNode2D;
            _target.TriggerInteraction(ActorContext.Actor, (InteractionState)_actionState, data);
            NotifManager.Instance.NotifyInteractionStarted(ActorContext.Actor);
        }

        protected override void ExecuteExit()
        {
            ActorContext.EmitSignal(ActorTag2D.SignalName.InteractionEnded);
            _target.StopInteraction();
            NotifManager.Instance.NotifyInteractionEnded(ActorContext.Actor);
        }

        public override void Update(double delta)
        {
            _negotiationTimer -= (float)delta;

            if (_negotiationTimer <= 0)
            {
                DetermineOutcome();
            }
        }

        private void DetermineOutcome()
        {
            ResourceStat targetResource = ResourceManager.Instance.GetResource(_resourceType, _target);
            float relationshipLevel = ActorContext.Memorizer.GetActorRelationship(_target);
            float baseProbability = ActorData.GetBasePetitionProbability(relationshipLevel);

            float excess = targetResource.Amount - targetResource.LowerThreshold;
            float normalRange = targetResource.UpperThreshold - targetResource.LowerThreshold;

            // * Pertains to how much the target has in excess of the lower threshold
            float resourceFactor = excess / normalRange;
            resourceFactor = Math.Clamp(resourceFactor, 0, 1);

            // * Base probability is the minimum probability
            // * It can be increased according to the resource weight and factor
            // * The higher the weight, the lower the additional probability
            float adjustedProbability = baseProbability + (1 - baseProbability) *
                (1 - targetResource.Weight) * resourceFactor;

            bool isAccepted = GD.Randf() < adjustedProbability;
            OnPetitionAnswered(_target, isAccepted);
        }

        private int CalculateAmount()
        {
            ResourceStat ownerResource = ResourceManager.Instance.GetResource(_resourceType, ActorContext.Actor);
            ResourceStat targetResource = ResourceManager.Instance.GetResource(_resourceType, _target);
            return CommonUtils.CalculateSkewedAmount(ownerResource, 0.8f, 2, targetResource.Amount);
        }

        private void OnPetitionAnswered(ActorTag2D source, bool isAccepted)
        {
            if (source != _target) return;

            _isAccepted = isAccepted;

            if (isAccepted)
            {
                ResourceManager.Instance.TranserResources(
                    _target,
                    ActorContext.Actor,
                    _resourceType,
                    _amount
                );
                _target.Memorizer.UpdateLastPetitionResource(ActorContext.Actor, _resourceType);
            }

            ActorContext.Memorizer.UpdateRelationship(
                _target,
                isAccepted ? CompanionshipIncrease : CompanionshipDecrease
            );

            ActorContext.Executor.FinishAction();
        }
    }
}