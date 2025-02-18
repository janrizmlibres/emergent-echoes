using System;
using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.States
{
    public enum Waypoint
    {
        Lateral,
        Omni
    }

    public enum EngageOutcome
    {
        TargetBusy,
        TargetAvailable,
        DurationExceeded
    }

    public class EngageState : BaseState, INavigationState
    {
        public const ActionState ActionStateValue = ActionState.Engage;

        private readonly ActorTag2D _target;
        private readonly Waypoint _waypoint;

        private bool _isTargetReached = false;
        private float _navigationTimer = 15;

        /// <summary>
        /// Event triggered when the state is completed.
        /// </summary>
        public event Action<EngageOutcome> CompleteState;

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

        public override void Subscribe()
        {
            _target.NotifManager.InteractionStarted += CompleteWithTargetBusy;
        }

        /// <summary>
        /// Called when the state is entered.
        /// </summary>
        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} EngageState Enter");

            _owner.Sensor.SetTaskRecord(_actionType, ActionStateValue);

            Error result = _owner.EmitSignal(
                NPCAgent2D.SignalName.ActionStateEntered,
                Variant.From(ActionStateValue),
                new Array<Variant>()
            );
            DebugTool.Assert(result != Error.Unavailable, "Signal emitted error");
        }

        public override void Update(double delta)
        {
            if (!_isTargetReached) return;

            _navigationTimer -= (float)delta;

            if (_navigationTimer <= 0)
            {
                CompleteState?.Invoke(EngageOutcome.DurationExceeded);
            }
        }

        public override void Unsubscribe()
        {
            _target.NotifManager.InteractionStarted -= CompleteWithTargetBusy;
        }

        /// <summary>
        /// Called when the state is exited.
        /// </summary>
        public override void Exit()
        {
            Error result = _owner.EmitSignal(
                NPCAgent2D.SignalName.ActionStateExited,
                Variant.From(ActionStateValue),
                new Array<Variant>()
            );
            DebugTool.Assert(result != Error.Unavailable, "Signal emitted error");
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

        public bool OnNavigationComplete()
        {
            _isTargetReached = true;

            // Get current position and target waypoint
            Vector2 currentPos = _owner.Parent.GlobalPosition;
            Vector2 targetWaypoint = GetTargetPosition();

            // Verify we're actually at the waypoint (within reasonable distance)
            float distanceToWaypoint = currentPos.DistanceTo(targetWaypoint);
            if (distanceToWaypoint < 5) // Adjust threshold as needed
            {
                CompleteState?.Invoke(EngageOutcome.TargetAvailable);
                return true;
            }

            return false;
        }

        private void CompleteWithTargetBusy()
        {
            CompleteState?.Invoke(EngageOutcome.TargetBusy);
        }
    }
}