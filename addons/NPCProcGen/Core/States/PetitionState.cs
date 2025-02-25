using System;
using Godot;
using Godot.Collections;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Internal;

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

        public override void Subscribe()
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
                Data = new Array<Variant> { _target.GetParent<Node2D>() }
            };
        }

        protected override ExitParameters GetExitData()
        {
            return new ExitParameters
            {
                Data = new Array<Variant>
                {
                    _target.GetParent<Node2D>(),
                    Variant.From(_resourceType),
                    _amount,
                    _isAccepted,
                    _isAccepted ? CompanionshipIncrease : CompanionshipDecrease,
                }
            };
        }

        protected override void ExecuteEnter()
        {
            Array<Variant> data = new()
            {
                _actorContext.ActorNode2D,
                Variant.From(_resourceType),
                _amount
            };

            _target.TriggerInteraction(_actorContext.Actor, (InteractState)_actionState, data);
            NotifManager.Instance.NotifyInteractionStarted(_actorContext.Actor);
        }

        protected override void ExecuteExit()
        {
            _target.StopInteraction();
            NotifManager.Instance.NotifyInteractionEnded(_actorContext.Actor);
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
            ResourceStat targetResource = ResourceManager.Instance.GetResource(_target, _resourceType);
            float relationshipLevel = _actorContext.Memorizer.GetActorRelationship(_target);
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
            ResourceStat ownerResource = ResourceManager.Instance.GetResource(_actorContext.Actor, _resourceType);
            ResourceStat targetResource = ResourceManager.Instance.GetResource(_target, _resourceType);
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
                    _actorContext.Actor,
                    _resourceType,
                    _amount
                );
                _target.Memorizer.UpdateLastPetitionResource(_actorContext.Actor, _resourceType);
            }

            _actorContext.Memorizer.UpdateRelationship(
                _target,
                isAccepted ? CompanionshipIncrease : CompanionshipDecrease
            );

            _actorContext.Executor.FinishAction();
        }
    }
}