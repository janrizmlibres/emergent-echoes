using System;
using Godot;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.States
{
    /// <summary>
    /// Represents a state where the NPC wanders around.
    /// </summary>
    public class WanderState : BaseState, INavigationState
    {
        private const float WanderRadius = 100;
        private const int Min = 5;
        private const int Max = 10;

        private readonly ActorTag2D _target;
        private Vector2 _targetPosition;

        // TODO: Should be higher in final implementation
        private float _maxDuration = 30;
        private float _wanderInterval;
        private float _timer = 0;
        private bool _isWandering = false;

        /// <summary>
        /// Event triggered when the state is completed.
        /// </summary>
        public event Action<bool> CompleteState;

        /// <summary>
        /// Initializes a new instance of the <see cref="WanderState"/> class.
        /// </summary>
        /// <param name="owner">The owner of the state.</param>
        /// <param name="target">The target actor.</param>
        public WanderState(NPCAgent2D owner, ActorTag2D target) : base(owner)
        {
            _target = target;
            _targetPosition = owner.Parent.GlobalPosition;
            _wanderInterval = CommonUtils.Rnd.Next(Min, Max);
        }

        /// <summary>
        /// Called when the state is entered.
        /// </summary>
        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} WanderState Enter");
            _owner.EmitSignal(NPCAgent2D.SignalName.ActionStateEntered, Variant.From(ActionState.Wander));
            _owner.NotifManager.NavigationComplete += OnNavigationComplete;
            _owner.NotifManager.ActorDetected += OnActorDetected;
        }

        /// <summary>
        /// Called when the state is exited.
        /// </summary>
        public override void Exit()
        {
            _owner.EmitSignal(NPCAgent2D.SignalName.ActionStateExited, Variant.From(ActionState.Wander));
            _owner.NotifManager.NavigationComplete -= OnNavigationComplete;
            _owner.NotifManager.ActorDetected -= OnActorDetected;
        }

        /// <summary>
        /// Updates the state.
        /// </summary>
        /// <param name="delta">The time elapsed since the last update.</param>
        public override void Update(double delta)
        {
            if (!_isWandering)
            {
                _timer += (float)delta;

                if (_timer >= _wanderInterval)
                {
                    _timer = 0;
                    _wanderInterval = CommonUtils.Rnd.Next(Min, Max);
                    _targetPosition = CommonUtils.GetRandomPosInCircularArea(
                        _owner.Parent.GlobalPosition, WanderRadius
                    );
                    _isWandering = true;
                }
            }

            _maxDuration -= (float)delta;

            if (_maxDuration <= 0)
            {
                OnCompleteState(true);
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
            return _targetPosition;
        }

        /// <summary>
        /// Handles the detection of an actor.
        /// </summary>
        /// <param name="target">The detected actor.</param>
        private void OnActorDetected(ActorTag2D target)
        {
            if (target == _target)
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
        }
    }
}