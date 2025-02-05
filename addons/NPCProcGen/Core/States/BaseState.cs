using Godot;
using NPCProcGen.Core.Components.Enums;

namespace NPCProcGen.Core.States
{
    /// <summary>
    /// Interface for navigation states.
    /// </summary>
    public interface INavigationState
    {
        /// <summary>
        /// Checks if the NPC is navigating.
        /// </summary>
        /// <returns>True if navigating, otherwise false.</returns>
        public bool IsNavigating();

        /// <summary>
        /// Gets the target position for navigation.
        /// </summary>
        /// <returns>The target position as a Vector2.</returns>
        public Vector2 GetTargetPosition();
    }

    /// <summary>
    /// Abstract base class for NPC states.
    /// </summary>
    public abstract class BaseState
    {
        /// <summary>
        /// The owner NPC agent.
        /// </summary>
        protected readonly NPCAgent2D _owner;
        protected readonly ActionType _actionType;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseState"/> class.
        /// </summary>
        /// <param name="owner">The owner NPC agent.</param>
        public BaseState(NPCAgent2D owner, ActionType action)
        {
            _owner = owner;
            _actionType = action;
        }

        /// <summary>
        /// Called when the state is entered.
        /// </summary>
        public virtual void Enter() { }

        /// <summary>
        /// Called to update the state.
        /// </summary>
        /// <param name="delta">The time elapsed since the last update.</param>
        public virtual void Update(double delta) { }

        /// <summary>
        /// Called when the state is exited.
        /// </summary>
        public virtual void Exit() { }

        protected bool IsActionSocial()
        {
            return _actionType == ActionType.Petition || _actionType == ActionType.Socialize;
        }
    }
}