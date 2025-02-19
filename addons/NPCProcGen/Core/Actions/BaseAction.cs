using NPCProcGen.Core.States;
using Godot;
using System;
using NPCProcGen.Core.Components.Enums;

namespace NPCProcGen.Core.Actions
{
    public interface IInteractionAction
    {
        public void Subscribe();
        public void Unsubscribe();
    }

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
        public void TransitionTo(BaseState newState)
        {
            _currentState?.Unsubscribe();
            _currentState?.Exit();
            _currentState = newState;
            _currentState?.Subscribe();
            _currentState?.Enter();
        }

        public void ClearState()
        {
            _currentState?.Unsubscribe();
            _currentState = null;
        }

        public void InterruptAction()
        {
            _owner.NotifManager.NotifyInteractionEnded();
            _owner.EmitSignal(
                ActorTag2D.SignalName.EventTriggered,
                Variant.From(EventType.TargetInterrupted)
            );

            ClearState();
            ActionComplete?.Invoke();
        }

        /// <summary>
        /// Completes the current action.
        /// </summary>
        /// <param name="actionType">The type of action that was completed.</param>
        public void CompleteAction()
        {
            TransitionTo(null);
            ActionComplete?.Invoke();
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
        /// Gets the target position for the current action.
        /// </summary>
        /// <returns>The target position as a <see cref="Vector2"/>.</returns>
        public Vector2 GetTargetPosition()
        {
            return (_currentState as INavigationState)?.GetTargetPosition()
                ?? _owner.Parent.GlobalPosition;
        }

        public bool CompleteNavigation()
        {
            return (_currentState as INavigationState)?.OnNavigationComplete() ?? false;
        }

        public void CompleteConsumption()
        {
            (_currentState as EatState)?.OnConsumptionComplete();
        }

        public void OnActorDetected(ActorTag2D actor)
        {
            (_currentState as IActorDetectionState)?.OnActorDetected(actor);
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