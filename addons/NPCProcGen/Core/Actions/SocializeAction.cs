using Godot;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    /// <summary>
    /// Represents an action to socialize with another actor.
    /// </summary>
    public class SocializeAction : BaseAction
    {
        public const ActionType ActionTypeValue = ActionType.Socialize;

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
            _seekState.CompleteState += (ActorTag2D partner) =>
            {
                MoveState moveState = new(_owner, ActionTypeValue, partner.Parent);
                TalkState talkState = new(_owner, ActionTypeValue, partner);

                moveState.CompleteState += (_) => TransitionTo(talkState);
                talkState.CompleteState += () => CompleteAction();

                TransitionTo(moveState);
            };
        }

        public override void Update(double delta)
        {
            _currentState?.Update(delta);
        }

        public override void Run()
        {
            CommonUtils.EmitSignal(
                _owner,
                NPCAgent2D.SignalName.ExecutionStarted,
                Variant.From(ActionTypeValue)
            );

            if (_owner.IsAnyActorInRange())
            {
                ActorTag2D actor = _owner.GetRandomActorInRange();

                MoveState moveState = new(_owner, ActionTypeValue, actor.Parent);
                TalkState talkState = new(_owner, ActionTypeValue, actor);

                moveState.CompleteState += (_) => TransitionTo(talkState);
                talkState.CompleteState += () => CompleteAction();

                TransitionTo(moveState);
            }
            else
            {
                TransitionTo(_seekState);
            }
        }
    }
}