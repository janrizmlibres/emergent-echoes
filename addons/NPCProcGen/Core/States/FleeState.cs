using System;
using Godot;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.States
{
    /// <summary>
    /// Represents the state where an NPC agent flees to a random position.
    /// </summary>
    public class FleeState : BaseState, INavigationState
    {
        private static readonly float _minDistance = 200;
        private static readonly float _maxDistance = 400;

        private Vector2 _target;

        /// <summary>
        /// Event triggered when the state is completed.
        /// </summary>
        public event Action CompleteState;

        /// <summary>
        /// Initializes a new instance of the <see cref="FleeState"/> class.
        /// </summary>
        /// <param name="owner">The NPC agent owning this state.</param>
        public FleeState(NPCAgent2D owner) : base(owner) { }

        /// <summary>
        /// Called when the state is entered.
        /// </summary>
        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} FleeState Enter");
            _owner.EmitSignal(NPCAgent2D.SignalName.ActionStateEntered, Variant.From(ActionState.Flee));
            _target = CommonUtils.GetRandomPosInCircularArea(_owner.Parent.GlobalPosition, _maxDistance, _minDistance);
            _owner.NotifManager.NavigationComplete += OnNavigationComplete;
        }

        /// <summary>
        /// Called when the state is exited.
        /// </summary>
        public override void Exit()
        {
            _owner.EmitSignal(NPCAgent2D.SignalName.ActionStateExited, Variant.From(ActionState.Flee));
            _owner.NotifManager.NavigationComplete -= OnNavigationComplete;
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
        /// <returns>The target position.</returns>
        public Vector2 GetTargetPosition()
        {
            return _target;
        }

        private void OnNavigationComplete()
        {
            CompleteState?.Invoke();
        }
    }
}