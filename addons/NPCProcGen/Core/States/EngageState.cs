using System;
using Godot;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.States
{
    public class EngageState : BaseState, INavigationState
    {
        public const ActionState ActionStateValue = ActionState.Engage;

        private readonly ActorTag2D _target;
        private readonly Waypoint _waypoint;

        /// <summary>
        /// Event triggered when the state is completed.
        /// </summary>
        public event Action<bool> CompleteState;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveState"/> class with a target node.
        /// </summary>
        /// <param name="owner">The NPC agent owning this state.</param>
        /// <param name="target">The target node to move to.</param>
        /// <param name="lastKnownPosition">The last known position of the target.</param>
        public EngageState(NPCAgent2D owner, ActionType action, ActorTag2D target,
            Waypoint waypoint) : base(owner, action)
        {
            _target = target;
            _waypoint = waypoint;
        }

        /// <summary>
        /// Called when the state is entered.
        /// </summary>
        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} EngageState Enter");

            _owner.NotifManager.NavigationComplete += OnNavigationComplete;
            _target.NotifManager.InteractionStarted += OnTargetInteractionStarted;

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
            _owner.NotifManager.NavigationComplete -= OnNavigationComplete;
            _target.NotifManager.InteractionStarted -= OnTargetInteractionStarted;

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
            if (_waypoint == Waypoint.Lateral)
                return _target.GetLateralWaypoint(_owner);

            if (_waypoint == Waypoint.Omni)
                return _target.GetOmniDirectionalWaypoint(_owner);

            throw new InvalidOperationException("Invalid waypoint type.");
        }

        private void OnTargetInteractionStarted()
        {
            bool isTargetBusy = true;
            CompleteState?.Invoke(isTargetBusy);
        }

        private void OnNavigationComplete()
        {
            bool isTargetBusy = false;
            CompleteState?.Invoke(isTargetBusy);
        }
    }
}