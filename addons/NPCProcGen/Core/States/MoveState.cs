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

        private readonly Node2D _targetNode;
        private Vector2? _movePosition;

        private readonly bool _isStealing;
        private bool _targetFound = false;

        private readonly ActorTag2D _targetActor;

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
        public MoveState(
            NPCAgent2D owner,
            ActionType action,
            Node2D target,
            Vector2? lastKnownPosition = null,
            bool isStealing = false)
            : base(owner, action)
        {
            _targetNode = target;
            _isStealing = isStealing;
            _movePosition = lastKnownPosition;

            _targetActor = target.GetChildren().OfType<ActorTag2D>().FirstOrDefault();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveState"/> class with a target position.
        /// </summary>
        /// <param name="owner">The NPC agent owning this state.</param>
        /// <param name="targetPosition">The target position to move to.</param>
        public MoveState(NPCAgent2D owner, ActionType action, Vector2 targetPosition)
            : base(owner, action)
        {
            _movePosition = targetPosition;
        }

        /// <summary>
        /// Called when the state is entered.
        /// </summary>
        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} MoveState Enter");
            _owner.NotifManager.NavigationComplete += OnNavigationComplete;
            _owner.NotifManager.ActorDetected += OnActorDetected;
            _owner.Sensor.SetTaskRecord(_owner, _actionType, ActionStateValue);
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
            _owner.NotifManager.ActorDetected -= OnActorDetected;
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
            // If only move position is set OR both move position and target node are set but target
            // is not found, return move position
            if (_targetNode == null || _movePosition != null && !_targetFound)
                return _movePosition.Value;

            // If target node is not an actor, return exact node position
            if (_targetActor == null) return _targetNode.GlobalPosition;

            // If owner is stealing, return rear marker position of target actor,
            // otherwise return offset position
            return _isStealing ? _targetActor.GetRearPosition() :
                CommonUtils.GetInteractionPosition(_owner, _targetActor);
        }

        private bool IsSeeking()
        {
            return _movePosition != null;
        }

        private void OnNavigationComplete()
        {
            if (!IsActionSocial() || !_owner.Sensor.IsActorBusy(_targetActor))
            {
                CompleteState?.Invoke(_targetFound);
            }
        }

        private void OnActorDetected(ActorTag2D target)
        {
            if (target.Parent == _targetNode && IsSeeking())
            {
                _targetFound = true;
            }
        }
    }
}