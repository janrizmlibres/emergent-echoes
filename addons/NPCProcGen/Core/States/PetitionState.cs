using System;
using Godot;
using Godot.Collections;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Actions;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.States
{
    public class PetitionState : BaseState
    {
        public const ActionState ActionStateValue = ActionState.Petition;

        private const float NegotiationDuration = 15;
        private const int CompanionshipIncrease = 3;
        private const int CompanionshipDecrease = -1;

        private readonly ActorTag2D _target;
        private readonly ResourceType _resourceType;
        private readonly int _amount;

        private bool _isAccepted;

        private float _negotiationTimer = NegotiationDuration;

        public event Action CompleteState;

        public PetitionState(NPCAgent2D owner, ActionType action, ActorTag2D target, ResourceType type)
            : base(owner, action)
        {
            _target = target;
            _resourceType = type;
            _amount = CalculateAmount();
        }

        public override void Subscribe()
        {
            _owner.NotifManager.PetitionAnswered += OnPetitionAnswered;
        }

        public override void Enter()
        {
            // GD.Print($"{_owner.Parent.Name} PetitionState Enter");

            if (_target is NPCAgent2D npc)
            {
                npc.AddAction(new InteractAction(npc, _owner));
            }
            else
            {
                _target.NotifManager.NotifyInteractionStarted();
                _target.Sensor.SetTaskRecord(ActionType.Interact, ActionState.Interact);

                Array<Variant> targetData = new()
                {
                    _owner.Parent,
                    Variant.From(_resourceType),
                    _amount,
                };

                CommonUtils.EmitSignal(
                    _target,
                    ActorTag2D.SignalName.InteractionStarted,
                    Variant.From((InteractState)ActionStateValue),
                    targetData
                );
            }

            _owner.NotifManager.NotifyInteractionStarted();
            _owner.Sensor.SetTaskRecord(_actionType, ActionStateValue);

            Array<Variant> ownerData = new() { _target.Parent };

            CommonUtils.EmitSignal(
                _owner,
                NPCAgent2D.SignalName.ActionStateEntered,
                Variant.From(ActionStateValue),
                ownerData
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

        public override void Unsubscribe()
        {
            _owner.NotifManager.PetitionAnswered -= OnPetitionAnswered;
        }

        public override void Exit()
        {
            _owner.NotifManager.NotifyInteractionEnded();
            _owner.Sensor.ClearTaskRecord();

            Array<Variant> data = new()
            {
                _target.Parent,
                Variant.From(_resourceType),
                _amount,
                _isAccepted,
                _isAccepted ? CompanionshipIncrease : CompanionshipDecrease,
            };

            CommonUtils.EmitSignal(
                _owner,
                NPCAgent2D.SignalName.ActionStateExited,
                Variant.From(ActionStateValue),
                data
            );

            if (_target is NPCAgent2D npc)
            {
                npc.EndAction();
            }
            else
            {
                _target.NotifManager.NotifyInteractionEnded();
                _target.Sensor.ClearTaskRecord();

                CommonUtils.EmitSignal(
                    _target,
                    ActorTag2D.SignalName.InteractionEnded
                );
            }
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
            _isAccepted = isAccepted;

            if (isAccepted)
            {
                ResourceManager.Instance.TranserResources(_target, _owner, _resourceType, _amount);
                _target.Memorizer.UpdateLastPetitionResource(_owner, _resourceType);
            }

            _owner.Memorizer.UpdateRelationship(
                _target,
                isAccepted ? CompanionshipIncrease : CompanionshipDecrease
            );
            CompleteState?.Invoke();
        }
    }
}