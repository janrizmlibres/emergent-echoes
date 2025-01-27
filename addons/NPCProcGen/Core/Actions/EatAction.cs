using Godot;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    /// <summary>
    /// Represents an action where an NPC agent attempts to eat a resource.
    /// </summary>
    public class EatAction : BaseAction
    {
        private readonly EatState _eatState;

        /// <summary>
        /// Initializes a new instance of the <see cref="EatAction"/> class.
        /// </summary>
        /// <param name="owner">The NPC agent performing the action.</param>
        public EatAction(NPCAgent2D owner) : base(owner)
        {
            _eatState = new EatState(owner);
            _eatState.CompleteState += () => CompleteAction(ActionType.Eat);
        }

        public override void Update(double delta)
        {
            _currentState?.Update(delta);
        }

        public override void Run()
        {
            _owner.EmitSignal(NPCAgent2D.SignalName.ExecutionStarted, Variant.From(ActionType.Eat));
            TransitionTo(_eatState);
        }
    }
}