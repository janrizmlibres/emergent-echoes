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
        public const ActionState ActionStateValue = ActionState.Talk;

        private const int MinDuration = 10;
        private const int MaxDuration = 20;
        private const int MinCompanionshipIncrease = 20;
        private const int MaxCompanionshipIncrease = 40;

        private readonly float _companionshipIncrease;

        private readonly ActorTag2D _partner;

        public event Action CompleteState;

        private float _duration;

        /// <summary>
        /// Initializes a new instance of the <see cref="TalkState"/> class.
        /// </summary>
        /// <param name="owner">The owner of the state.</param>
        public TalkState(NPCAgent2D owner, ActionType action, ActorTag2D partner)
            : base(owner, action)
        {
            _partner = partner;
            _duration = GD.RandRange(MinDuration, MaxDuration);
            _companionshipIncrease = ComputeIncrease();
        }

        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} TalkState Enter");

            Array<Variant> ownerData = new() { _partner.Parent };
            Array<Variant> partnerData = new() { _owner.Parent };

            _owner.NotifManager.NotifyInteractionStarted();
            _partner.NotifManager.NotifyInteractionStarted();

            _owner.Sensor.SetTaskRecord(_actionType, ActionStateValue);
            _partner.Sensor.SetTaskRecord(_actionType, ActionStateValue);

            CommonUtils.EmitSignal(
                _owner,
                NPCAgent2D.SignalName.ActionStateEntered,
                Variant.From(ActionStateValue),
                ownerData
            );
            CommonUtils.EmitSignal(
                _partner,
                ActorTag2D.SignalName.InteractionStarted,
                Variant.From((InteractState)ActionStateValue),
                partnerData
            );
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
            _owner.NotifManager.NotifyInteractionEnded();
            _partner.NotifManager.NotifyInteractionEnded();
            _partner.Sensor.ClearTaskRecord();

            Array<Variant> data = new()
            {
                _partner.Parent,
                _companionshipIncrease
            };

            CommonUtils.EmitSignal(
                _owner,
                NPCAgent2D.SignalName.ActionStateExited,
                Variant.From(ActionStateValue),
                data
            );
            CommonUtils.EmitSignal(_partner, ActorTag2D.SignalName.InteractionEnded);

            if (_partner is NPCAgent2D npc && !npc.IsActive())
            {
                CommonUtils.EmitSignal(
                    _partner,
                    NPCAgent2D.SignalName.ExecutionEnded
                );
            }
        }

        private float ComputeIncrease()
        {
            float scaler = (_duration - MinDuration) / (MaxDuration - MinDuration);
            float increaseRange = MaxCompanionshipIncrease - MinCompanionshipIncrease;
            float amount = MinCompanionshipIncrease + scaler * increaseRange;
            return Math.Clamp(amount, MinCompanionshipIncrease, MaxCompanionshipIncrease);
        }

        private void EndInteraction()
        {
            // Calculate the amount of companionship to increase based on duration using
            // linear interpolation
            ResourceManager.Instance.ModifyResource(
                _owner,
                ResourceType.Companionship,
                _companionshipIncrease
            );
            ResourceManager.Instance.ModifyResource(
                _partner,
                ResourceType.Companionship,
                _companionshipIncrease
            );

            _owner.Memorizer.UpdateRelationship(_partner, 1);
            _partner.Memorizer.UpdateRelationship(_owner, 1);

            CompleteState?.Invoke();
        }
    }
}