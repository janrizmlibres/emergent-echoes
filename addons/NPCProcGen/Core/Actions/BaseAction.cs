using NPCProcGen.Core.States;
using Godot;
using System;
using NPCProcGen.Core.Components.Enums;

namespace NPCProcGen.Core.Actions
{
    /// <summary>
    /// Abstract base class for actions performed by an NPC agent.
    /// </summary>
    public abstract class BaseAction
    {
        /// <summary>
        /// The NPC agent that owns this action.
        /// </summary>
        protected readonly NPCAgent2D _owner;

        /// <summary>
        /// The current state of the action.
        /// </summary>
        protected BaseState _currentState;

        /// <summary>
        /// Event triggered when the action is complete.
        /// </summary>
        public event Action ActionComplete;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAction"/> class.
        /// </summary>
        /// <param name="owner">The NPC agent that owns this action.</param>
        public BaseAction(NPCAgent2D owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// Transitions to a new state.
        /// </summary>
        /// <param name="newState">The new state to transition to.</param>
        protected void TransitionTo(BaseState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState?.Enter();
        }

        /// <summary>
        /// Completes the current action.
        /// </summary>
        /// <param name="actionType">The type of action that was completed.</param>
        protected void CompleteAction()
        {
            TransitionTo(null);
            ActionComplete?.Invoke();
            _owner.EmitSignal(NPCAgent2D.SignalName.ExecutionEnded);

            _owner.Sensor.ClearTaskRecord();
            _owner.Sensor.ClearPetitionResourceType();
        }

        /// <summary>
        /// Gets the target position for the current action.
        /// </summary>
        /// <returns>The target position as a <see cref="Vector2"/>.</returns>
        public Vector2 GetTargetPosition()
        {
            return (_currentState as INavigationState)?.GetTargetPosition() ?? _owner.Parent.GlobalPosition;
        }

        /// <summary>
        /// Determines whether the NPC is currently navigating.
        /// </summary>
        /// <returns><c>true</c> if the NPC is navigating; otherwise, <c>false</c>.</returns>
        public bool IsNavigating()
        {
            return _currentState is INavigationState state && state.IsNavigating();
        }

        /// <summary>
        /// Updates the action.
        /// </summary>
        /// <param name="delta">The time elapsed since the last update.</param>
        public abstract void Update(double delta);

        /// <summary>
        /// Runs the action.
        /// </summary>
        public abstract void Run();
    }
}