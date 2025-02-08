using System;
using System.Linq;
using Godot;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.States
{
    /// <summary>
    /// Represents the state where an NPC agent moves to a target position.
    /// </summary>
    public class MoveState : BaseState, INavigationState
    {
        public const ActionState ActionStateValue = ActionState.Move;

        private Vector2 _movePosition;

        /// <summary>
        /// Event triggered when the state is completed.
        /// </summary>
        public event Action CompleteState;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveState"/> class with a target node.
        /// </summary>
        /// <param name="owner">The NPC agent owning this state.</param>
        /// <param name="target">The target node to move to.</param>
        /// <param name="lastKnownPosition">The last known position of the target.</param>
        public MoveState(NPCAgent2D owner, ActionType action, Vector2 movePosition) : base(owner, action)
        {
            _movePosition = movePosition;
        }

        /// <summary>
        /// Called when the state is entered.
        /// </summary>
        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} MoveState Enter");

            _owner.Sensor.SetTaskRecord(_actionType, ActionStateValue);

            CommonUtils.EmitSignal(
                _owner,
                NPCAgent2D.SignalName.ActionStateEntered,
                Variant.From(ActionStateValue)
            );
        }

        /// <summary>
        /// Called when the state is exited.
        /// </summary>
        public override void Exit()
        {
            CommonUtils.EmitSignal(
                _owner,
                NPCAgent2D.SignalName.ActionStateExited,
                Variant.From(ActionStateValue)
            );
        }

        /// <summary>
        /// Determines whether the agent is currently navigating.
        /// </summary>
        /// <returns>True if the agent is navigating; otherwise, false.</returns>
        public bool IsNavigating()
        {
            return true;
        }

        /// <summary>
        /// Gets the target position for navigation.
        /// </summary>
        /// <returns>The global position of the target.</returns>
        public Vector2 GetTargetPosition()
        {
            return _movePosition;
        }

        public void OnNavigationComplete()
        {
            CompleteState?.Invoke();
        }
    }
}