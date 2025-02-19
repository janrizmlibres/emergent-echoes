using Godot;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    /// <summary>
    /// Represents an action to socialize with another actor.
    /// </summary>
    public class SocializeAction : BaseAction, IInteractionAction
    {
        public const ActionType ActionTypeValue = ActionType.Socialize;

        private ActorTag2D _target = null;

        private SeekState _seekState;

        /// <summary>
        /// Initializes a new instance of the <see cref="SocializeAction"/> class.
        /// </summary>
        /// <param name="owner">The owner of the action.</param>
        public SocializeAction(NPCAgent2D owner) : base(owner)
        {
            InitializeStates();
        }

        private void InitializeStates()
        {
            _seekState = new SeekState(_owner, ActionTypeValue);
            _seekState.CompleteState += partner => InitializeInteractStates(partner);
        }

        public void Subscribe() { }

        public void Unsubscribe()
        {
            if (_target != null)
            {
                _target.NotifManager.ActorImprisoned -= InterruptAction;
            }
        }

        public override void Update(double delta)
        {
            _currentState?.Update(delta);
        }

        public override void Run()
        {
            Error result = _owner.EmitSignal(
                NPCAgent2D.SignalName.ExecutionStarted,
                Variant.From(ActionTypeValue)
            );
            DebugTool.Assert(result != Error.Unavailable, "Signal emitted error");

            if (_owner.IsAnyActorInRange())
            {
                ActorTag2D target = _owner.GetRandomActorInRange();
                InitializeInteractStates(target);
            }
            else
            {
                TransitionTo(_seekState);
            }
        }

        private void InitializeInteractStates(ActorTag2D partner)
        {
            _target = partner;
            _target.NotifManager.ActorImprisoned += InterruptAction;

            EngageState engageState = new(_owner, ActionTypeValue, partner, Waypoint.Lateral);
            WaitState waitState = new(_owner, ActionTypeValue, partner);
            TalkState talkState = new(_owner, ActionTypeValue, partner);

            engageState.CompleteState += outcome =>
            {
                switch (outcome)
                {
                    case EngageOutcome.DurationExceeded:
                        CompleteAction();
                        break;
                    case EngageOutcome.TargetBusy:
                        TransitionTo(waitState);
                        break;
                    case EngageOutcome.TargetAvailable:
                        TransitionTo(talkState);
                        break;
                }
            };
            waitState.CompleteState += () => TransitionTo(engageState);
            talkState.CompleteState += () => CompleteAction();

            TransitionTo(partner.Sensor.IsActorBusy() ? waitState : engageState);
        }
    }
}