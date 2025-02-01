using System;
using Godot;
using NPCProcGen.Core.Components.Enums;

namespace NPCProcGen.Core.States
{
    /// <summary>
    /// Represents the state where an NPC agent moves to a target position.
    /// </summary>
    public class MoveState : BaseState, INavigationState
    {
        private readonly Node2D _target = null;
        private Vector2? _targetPosition = null;

        /// <summary>
        /// Event triggered when the state is completed.
        /// </summary>
        public event Action<bool> CompleteState;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveState"/> class with a target node.
        /// </summary>
        /// <param name="owner">The NPC agent owning this state.</param>
        /// <param name="target">The target node to move to.</param>
        /// <param name="lastPos">The last known position of the target.</param>
        public MoveState(NPCAgent2D owner, Node2D target, Vector2? lastPos = null)
            : base(owner)
        {
            _target = target;
            _targetPosition = lastPos;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveState"/> class with a target position.
        /// </summary>
        /// <param name="owner">The NPC agent owning this state.</param>
        /// <param name="targetPosition">The target position to move to.</param>
        public MoveState(NPCAgent2D owner, Vector2 targetPosition)
            : base(owner)
        {
            _targetPosition = targetPosition;
        }

        /// <summary>
        /// Called when the state is entered.
        /// </summary>
        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} MoveState Enter - Instance: {GetHashCode()}");
            _owner.EmitSignal(NPCAgent2D.SignalName.ActionStateEntered, Variant.From(ActionState.Move));
            _owner.NotifManager.NavigationComplete += OnNavigationComplete;
            _owner.NotifManager.ActorDetected += OnActorDetected;
        }

        /// <summary>
        /// Called when the state is exited.
        /// </summary>
        public override void Exit()
        {
            _owner.EmitSignal(NPCAgent2D.SignalName.ActionStateExited, Variant.From(ActionState.Move));
            _owner.NotifManager.NavigationComplete -= OnNavigationComplete;
            _owner.NotifManager.ActorDetected -= OnActorDetected;
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
            return _targetPosition ?? _target.GlobalPosition;
        }

        private void OnNavigationComplete()
        {
            OnCompleteState(false);
        }

        private void OnActorDetected(ActorTag2D target)
        {
            if (target.Parent == _target)
            {
                OnCompleteState(true);
            }
        }

        private void OnCompleteState(bool isActorDetected)
        {
            CompleteState?.Invoke(isActorDetected);
        }
    }
}