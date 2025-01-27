using System;
using Godot;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.States
{
    /// <summary>
    /// Represents the state of talking to another actor.
    /// </summary>
    public class TalkState : BaseState, INavigationState
    {
        private const int MinDuration = 10;
        private const int MaxDuration = 50;
        private const int MinCompanionshipIncrease = 5;
        private const int MaxCompanionshipIncrease = 20;

        public NPCAgent2D Partner { get; set; }

        public event Action CompleteState;

        private bool _isTalking = false;
        private float _duration;

        /// <summary>
        /// Initializes a new instance of the <see cref="TalkState"/> class.
        /// </summary>
        /// <param name="owner">The owner of the state.</param>
        public TalkState(NPCAgent2D owner, NPCAgent2D partner = null) : base(owner)
        {
            Partner = partner;
            _duration = CommonUtils.Rnd.Next(MinDuration, MaxDuration);

            if (Partner == null)
            {
                _owner.NotifManager.NotifyRandomActorRequested();
            }
        }

        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} TalkState Enter");
            _owner.EmitSignal(NPCAgent2D.SignalName.ActionStateEntered, Variant.From(ActionState.Talk));
            _owner.NotifManager.NavigationComplete += OnNavigationComplete;
        }

        public override void Update(double delta)
        {
            if (_isTalking)
            {
                _duration -= (float)delta;

                if (_duration <= 0)
                {
                    IncreaseCompanionshipAndEnd();
                }
            }
        }

        public override void Exit()
        {
            _owner.EmitSignal(NPCAgent2D.SignalName.ActionStateExited, Variant.From(ActionState.Talk));
            _owner.NotifManager.NavigationComplete -= OnNavigationComplete;
        }

        public bool IsNavigating()
        {
            return !_isTalking;
        }

        public Vector2 GetTargetPosition()
        {
            return Partner != null ? CommonUtils.GetInteractionPosition(_owner, Partner) : _owner.Parent.GlobalPosition;
        }

        private void OnNavigationComplete()
        {
            _isTalking = true;

            GD.Print($"{_owner.Parent.Name} is talking to {Partner.Parent.Name}");
            CommonUtils.SetFacingDirectionsAndNotify(_owner, Partner);
        }

        private void IncreaseCompanionshipAndEnd()
        {
            // * Calculate the amount of companionship to increase based on duration using linear interpolation
            float scaler = (_duration - MinDuration) / (MaxDuration - MinDuration);
            float increaseRange = MaxCompanionshipIncrease - MinCompanionshipIncrease;
            float amount = MinCompanionshipIncrease + scaler * increaseRange;
            amount = Math.Clamp(amount, MinCompanionshipIncrease, MaxCompanionshipIncrease);

            ResourceManager.Instance.IncreaseCompanionship(_owner, amount);
            ResourceManager.Instance.IncreaseCompanionship(Partner, amount);

            _owner.Memorizer.ModifyRelationship(Partner, 1);
            Partner.Memorizer.ModifyRelationship(_owner, 1);

            Partner.EmitSignal(ActorTag2D.SignalName.InteractionEnded);
            CompleteState?.Invoke();
        }
    }
}