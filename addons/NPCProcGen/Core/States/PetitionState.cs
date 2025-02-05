using System;
using Godot;
using Godot.Collections;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.States
{
    public class PetitionState : BaseState
    {
        public const ActionState ActionStateValue = ActionState.Petition;

        private const float NegotiationDuration = 15;

        private readonly ActorTag2D _target;
        private readonly ResourceType _resourceType;
        private readonly int _amount;

        private float _negotiationTimer = NegotiationDuration;

        public event Action CompleteState;

        public PetitionState(NPCAgent2D owner, ActionType action, ActorTag2D target, ResourceType type)
            : base(owner, action)
        {
            _target = target;
            _resourceType = type;
            _amount = CalculateAmount();
        }

        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} PetitionState Enter");

            Array<Variant> ownerData = new() { _target.Parent };
            Array<Variant> targetData = new()
            {
                _owner.Parent,
                Variant.From(_resourceType),
                _amount,
            };

            _owner.NotifManager.PetitionAnswered += OnPetitionAnswered;
            _target.NotifManager.NotifyInteractionStarted();
            _owner.Sensor.SetTaskRecord(_owner, _actionType, ActionStateValue);
            _target.Sensor.SetTaskRecord(_target, _actionType, ActionStateValue);

            CommonUtils.EmitSignal(
                _owner,
                NPCAgent2D.SignalName.ActionStateEntered,
                Variant.From(ActionStateValue),
                ownerData
            );
            CommonUtils.EmitSignal(
                _target,
                ActorTag2D.SignalName.InteractionStarted,
                Variant.From((InteractState)ActionStateValue),
                targetData
            );
        }

        public override void Update(double delta)
        {
            _negotiationTimer -= (float)delta;

            if (_negotiationTimer <= 0)
            {
                DetermineOutcome();
            }
        }

        public override void Exit()
        {
            GD.Print($"{_owner.Parent.Name} PetitionState Exit");
            _owner.NotifManager.PetitionAnswered -= OnPetitionAnswered;
            _target.NotifManager.NotifyInteractionEnded();
            _target.Sensor.ResetTaskRecord(_target);

            Array<Variant> data = new()
            {
                _target.Parent,
                Variant.From(_resourceType),
                _amount,
            };

            CommonUtils.EmitSignal(
                _owner,
                NPCAgent2D.SignalName.ActionStateExited,
                Variant.From(ActionStateValue),
                data
            );
            CommonUtils.EmitSignal(
                _target,
                ActorTag2D.SignalName.InteractionEnded
            );
        }

        private void DetermineOutcome()
        {
            ResourceStat targetResource = ResourceManager.Instance.GetResource(_target, _resourceType);
            float relationshipLevel = _owner.Memorizer.GetActorRelationship(_target);
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
            OnPetitionAnswered(isAccepted);
        }

        private int CalculateAmount()
        {
            ResourceStat ownerResource = ResourceManager.Instance.GetResource(_owner, _resourceType);
            ResourceStat targetResource = ResourceManager.Instance.GetResource(_target, _resourceType);
            return CommonUtils.CalculateSkewedAmount(ownerResource, 0.8f, 2, targetResource.Amount);
        }

        private void OnPetitionAnswered(bool isAccepted)
        {
            if (isAccepted)
            {
                ResourceManager.Instance.TranserResources(_target, _owner, _resourceType, _amount);
            }

            _owner.Memorizer.UpdateRelationship(_target, isAccepted ? 3 : -1);
            CompleteState?.Invoke();
        }
    }
}