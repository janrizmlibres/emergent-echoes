using Godot;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    /// <summary>
    /// Represents an action to socialize with another actor.
    /// </summary>
    public class SocializeAction : BaseAction
    {
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
            _seekState = new SeekState(_owner);
            _seekState.CompleteState += (ActorTag2D partner) =>
            {
                TalkState _talkState = new(_owner, partner);
                _talkState.CompleteState += () => CompleteAction(ActionType.Socialize);
                TransitionTo(_talkState);
            };
        }

        public override void Update(double delta)
        {
            _currentState?.Update(delta);
        }

        public override void Run()
        {
            _owner.EmitSignal(NPCAgent2D.SignalName.ExecutionStarted, Variant.From(ActionType.Socialize));

            if (_owner.IsAnyActorInRange())
            {
                TalkState _talkState = new(_owner, _owner.GetRandomActorInRange());
                _talkState.CompleteState += () => CompleteAction(ActionType.Socialize);
                TransitionTo(_talkState);
            }
            else
            {
                TransitionTo(_seekState);
            }
        }
    }
}