using System;
using Godot;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.States
{
    /// <summary>
    /// Represents the state of seeking a target.
    /// </summary>
    public class SeekState : BaseState, INavigationState
    {
        public const ActionState ActionStateValue = ActionState.Seek;

        private const float SeekRadius = 150;
        private const float IdleDuration = 10;

        private Vector2 _seekPosition;

        private bool _isMoving = false;
        private float _idleTimer = IdleDuration;

        public event Action<ActorTag2D> CompleteState;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeekState"/> class.
        /// </summary>
        /// <param name="owner">The owner of the state.</param>
        public SeekState(NPCAgent2D owner, ActionType action) : base(owner, action)
        {
            _seekPosition = owner.Parent.GlobalPosition;
        }

        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} SeekState Enter");
            _owner.NotifManager.NavigationComplete += OnNavigationComplete;
            _owner.NotifManager.ActorDetected += OnActorDetected;
            _owner.Sensor.SetTaskRecord(_owner, _actionType, ActionStateValue);
            CommonUtils.EmitSignal(
                _owner,
                NPCAgent2D.SignalName.ActionStateEntered,
                Variant.From(ActionStateValue)
            );
        }

        public override void Update(double delta)
        {
            if (_isMoving) return;

            _idleTimer -= (float)delta;

            if (_idleTimer <= 0)
            {
                _seekPosition = CommonUtils.GetRandomPosInCircularArea(
                    _owner.Parent.GlobalPosition,
                    SeekRadius
                );
                _idleTimer = IdleDuration;
                _isMoving = true;
            }
        }

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

        public bool IsNavigating()
        {
            return _isMoving;
        }

        public Vector2 GetTargetPosition()
        {
            return _seekPosition;
        }

        private void OnNavigationComplete()
        {
            _isMoving = false;
        }

        private void OnActorDetected(ActorTag2D actor)
        {
            CompleteState?.Invoke(actor);
        }
    }
}