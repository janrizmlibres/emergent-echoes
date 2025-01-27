using System;
using Godot;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.States
{
    public class PetitionState : BaseState, INavigationState
    {
        private const float NegotiationDuration = 15;

        private readonly ActorTag2D _target;
        private readonly ResourceType _resourceType;
        private readonly int _amount;
        private readonly bool _isTargetPlayer;

        private bool _isTargetReached = false;
        private float _negotiationTimer = 0;

        public event Action CompleteState;

        public PetitionState(NPCAgent2D owner, ActorTag2D target, ResourceType type)
            : base(owner)
        {
            _target = target;
            _resourceType = type;
            _isTargetPlayer = target is not NPCAgent2D;
            _amount = CalculateAmount();
        }

        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} PetitionState Enter");
            GD.Print($"Amount to petition: {_amount}");

            _owner.NotifManager.NavigationComplete += OnNavigationComplete;
            _owner.NotifManager.PetitionAnswered += OnPetitionAnswered;
            _owner.EmitSignal(NPCAgent2D.SignalName.ActionStateEntered, Variant.From(ActionState.Petition));
        }

        public override void Update(double delta)
        {
            // * Negotation only occurs when the target is an NPC and not the player
            if (!_isTargetReached && _isTargetPlayer) return;

            _negotiationTimer += (float)delta;

            if (_negotiationTimer >= NegotiationDuration)
            {
                DetermineOutcome();
            }
        }

        public override void Exit()
        {
            _owner.NotifManager.NavigationComplete -= OnNavigationComplete;
            _owner.NotifManager.PetitionAnswered -= OnPetitionAnswered;
            _owner.EmitSignal(NPCAgent2D.SignalName.ActionStateExited, Variant.From(ActionState.Petition));
        }

        public Vector2 GetTargetPosition()
        {
            return CommonUtils.GetInteractionPosition(_owner, _target);
        }

        public bool IsNavigating()
        {
            return !_isTargetReached;
        }

        private void DetermineOutcome()
        {
            ResourceStat targetResource = ResourceManager.Instance.GetResource(_target, _resourceType);
            float relationshipLevel = _owner.Memorizer.GetActorRelationship(_target);
            float baseProbability = GetBasePetitionProbability(relationshipLevel);

            float excess = targetResource.Amount - targetResource.LowerThreshold;
            float normalRange = targetResource.UpperThreshold - targetResource.LowerThreshold;

            // * Pertains to how much the target has in excess of the lower threshold
            float resourceFactor = excess / normalRange;
            resourceFactor = Math.Clamp(resourceFactor, 0, 1);

            // * Base probability is the minimum probability
            // * It can be increased according to the resource weight and factor
            // * The higher the weight, the lower the additional probability
            float adjustedProbability = baseProbability + (1 - baseProbability) * (1 - targetResource.Weight) * resourceFactor;

            GD.Print($"Petition success rate: {adjustedProbability}");
            GD.Print($"Lower threshold: {targetResource.LowerThreshold}");
            bool isAccepted = GD.Randf() < adjustedProbability;
            OnPetitionAnswered(isAccepted);
        }

        private static float GetBasePetitionProbability(float relationshipLevel)
        {
            return relationshipLevel switch
            {
                <= -26 => 0.05f,
                <= -16 => 0.20f,
                <= -6 => 0.30f,
                <= 4 => 0.40f,
                <= 14 => 0.60f,
                <= 24 => 0.80f,
                _ => 0.95f,
            };
        }

        private int CalculateAmount()
        {
            ResourceStat npcResource = ResourceManager.Instance.GetResource(_owner, _resourceType);
            ResourceStat targetResource = ResourceManager.Instance.GetResource(_target, _resourceType);

            // Calculate the deficiency for the NPC
            float deficiency = npcResource.LowerThreshold - npcResource.Amount;
            // Ensure deficiency is not below the minimum raise
            deficiency = Math.Max(npcResource.GetMinRaise(), deficiency);

            // Calculate a potential petition amount based on the weight
            float petitionAmount = deficiency * (1 + npcResource.Weight);

            // Ensure petition amount does not exceed the target's current resource amount
            petitionAmount = Math.Min(petitionAmount, targetResource.Amount);

            return (int)Math.Floor(petitionAmount);
        }

        private void OnNavigationComplete()
        {
            if (_isTargetReached) return;

            _isTargetReached = true;

            if (_isTargetPlayer)
            {
                _target.EmitSignal(ActorTag2D.SignalName.PetitionRequested, Variant.From(_resourceType), _amount);
                return;
            }

            GD.Print($"{_owner.Parent.Name} and {_target.Parent.Name} started negotiation");
            CommonUtils.SetFacingDirectionsAndNotify(_owner, _target);
        }

        private void OnPetitionAnswered(bool isAccepted)
        {
            GD.Print($"{_target.Parent.Name} {(isAccepted ? "accepted" : "rejected")} the petition from {_owner.Parent.Name}");

            if (isAccepted)
            {
                ResourceManager.Instance.TranserResources(_target, _owner, _resourceType, _amount);
                GD.Print($"Remaining resource: {ResourceManager.Instance.GetResource(_target, _resourceType).Amount}");
            }

            _target.EmitSignal(ActorTag2D.SignalName.InteractionEnded);
            _owner.Memorizer.ModifyRelationship(_target, isAccepted ? 3 : -1);
            CompleteState?.Invoke();
        }
    }
}