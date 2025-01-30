using System;
using Godot;
using Godot.Collections;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.States
{
    /// <summary>
    /// Represents the state of talking to another actor.
    /// </summary>
    public class TalkState : BaseState
    {
        private const int MinDuration = 10;
        private const int MaxDuration = 50;
        private const int MinCompanionshipIncrease = 5;
        private const int MaxCompanionshipIncrease = 20;

        private readonly ActorTag2D _partner;

        public event Action CompleteState;

        private float _duration;

        /// <summary>
        /// Initializes a new instance of the <see cref="TalkState"/> class.
        /// </summary>
        /// <param name="owner">The owner of the state.</param>
        public TalkState(NPCAgent2D owner, ActorTag2D partner) : base(owner)
        {
            _partner = partner;
            _duration = CommonUtils.Rnd.Next(MinDuration, MaxDuration);
        }

        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} TalkState Enter");

            Array<Variant> ownerData = new() { _partner.Parent };
            Array<Variant> partnerData = new() { _owner.Parent };

            _owner.EmitSignal(NPCAgent2D.SignalName.ActionStateEntered, Variant.From(ActionState.Talk), ownerData);
            (_partner as NPCAgent2D)?.EmitSignal(NPCAgent2D.SignalName.ActionStateEntered, Variant.From(ActionState.Talk), partnerData);
            _partner.EmitSignal(ActorTag2D.SignalName.InteractionStarted, Variant.From(ActionState.Talk), partnerData);
        }

        public override void Update(double delta)
        {
            _duration -= (float)delta;

            if (_duration <= 0)
            {
                EndInteraction();
            }
        }

        public override void Exit()
        {
            _owner.EmitSignal(NPCAgent2D.SignalName.ActionStateExited, Variant.From(ActionState.Talk));
            (_partner as NPCAgent2D)?.EmitSignal(NPCAgent2D.SignalName.ActionStateExited, Variant.From(ActionState.Talk));
            _partner.EmitSignal(ActorTag2D.SignalName.InteractionEnded);
        }

        private void EndInteraction()
        {
            // * Calculate the amount of companionship to increase based on duration using linear interpolation
            float scaler = (_duration - MinDuration) / (MaxDuration - MinDuration);
            float increaseRange = MaxCompanionshipIncrease - MinCompanionshipIncrease;
            float amount = MinCompanionshipIncrease + scaler * increaseRange;
            amount = Math.Clamp(amount, MinCompanionshipIncrease, MaxCompanionshipIncrease);

            ResourceManager.Instance.ModifyResource(_owner, ResourceType.Companionship, amount);
            ResourceManager.Instance.ModifyResource(_partner, ResourceType.Companionship, amount);

            _owner.Memorizer.UpdateRelationship(_partner, 1);
            _partner.Memorizer.UpdateRelationship(_owner, 1);

            CompleteState?.Invoke();
        }
    }
}