using Godot;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    /// <summary>
    /// Represents an action where an NPC agent attempts to eat a resource.
    /// </summary>
    public class EatAction : BaseAction
    {
        public const ActionType ActionTypeValue = ActionType.Eat;

        private EatState _eatState;

        /// <summary>
        /// Initializes a new instance of the <see cref="EatAction"/> class.
        /// </summary>
        /// <param name="owner">The NPC agent performing the action.</param>
        public EatAction(NPCAgent2D owner) : base(owner)
        {
            InitializeStates();
        }

        private void InitializeStates()
        {
            _eatState = new EatState(_owner, ActionTypeValue);
            _eatState.CompleteState += () => CompleteAction();
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
            TransitionTo(_eatState);
        }
    }
}