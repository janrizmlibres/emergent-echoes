using System;
using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.States
{
    /// <summary>
    /// Represents a state where the NPC wanders around.
    /// </summary>
    public class WanderState : BaseState, INavigationState
    {
        public const ActionState ActionStateValue = ActionState.Wander;

        private const float WanderRadius = 100;
        private const int MinInterval = 5;
        private const int MaxInterval = 10;
        private const int MaxDuration = 30;

        private readonly ActorTag2D _targetActor;
        private Vector2 _wanderPosition;
        private Vector2 _origin;

        private bool _isWandering = false;

        private float _durationTimer = MaxDuration;
        private float _idleTimer;

        /// <summary>
        /// Event triggered when the state is completed.
        /// </summary>
        public event Action<bool> CompleteState;

        /// <summary>
        /// Initializes a new instance of the <see cref="WanderState"/> class.
        /// </summary>
        /// <param name="owner">The owner of the state.</param>
        /// <param name="target">The target actor.</param>
        public WanderState(NPCAgent2D owner, ActionType action, ActorTag2D target)
            : base(owner, action)
        {
            _targetActor = target;
            _wanderPosition = owner.Parent.GlobalPosition;
            _idleTimer = CommonUtils.Rnd.Next(MinInterval, MaxInterval);
        }

        /// <summary>
        /// Called when the state is entered.
        /// </summary>
        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} WanderState Enter");

            _origin = _owner.Parent.GlobalPosition;

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
            // Bring back emit signal in refactor
        }

        /// <summary>
        /// Updates the state.
        /// </summary>
        /// <param name="delta">The time elapsed since the last update.</param>
        public override void Update(double delta)
        {
            _durationTimer -= (float)delta;

            if (_durationTimer <= 0)
            {
                OnCompleteState(true);
            }

            if (_isWandering) return;

            _idleTimer -= (float)delta;

            if (_idleTimer <= 0)
            {
                _wanderPosition = CommonUtils.GetRandomPosInCircularArea(_origin, WanderRadius);
                _idleTimer = CommonUtils.Rnd.Next(MinInterval, MaxInterval);
                _isWandering = true;
            }
        }

        /// <summary>
        /// Checks if the NPC is navigating.
        /// </summary>
        /// <returns>True if the NPC is navigating, otherwise false.</returns>
        public bool IsNavigating()
        {
            return _isWandering;
        }

        /// <summary>
        /// Gets the target position.
        /// </summary>
        /// <returns>The target position.</returns>
        public Vector2 GetTargetPosition()
        {
            return _wanderPosition;
        }

        /// <summary>
        /// Handles the detection of an actor.
        /// </summary>
        /// <param name="target">The detected actor.</param>
        private void OnActorDetected(ActorTag2D target)
        {
            if (target == _targetActor)
            {
                OnCompleteState(false);
            }
        }

        /// <summary>
        /// Handles the completion of navigation.
        /// </summary>
        private void OnNavigationComplete()
        {
            _isWandering = false;
        }

        /// <summary>
        /// Handles the completion of the state.
        /// </summary>
        /// <param name="durationReached">Indicates if the maximum duration was reached.</param>
        private void OnCompleteState(bool durationReached)
        {
            CompleteState?.Invoke(durationReached);
            Array<Variant> data = new() { durationReached };
            CommonUtils.EmitSignal(
                _owner,
                NPCAgent2D.SignalName.ActionStateExited,
                Variant.From(ActionStateValue),
                data
            );
        }
    }
}