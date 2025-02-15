using System;
using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.States
{
    /// <summary>
    /// Represents the state of seeking a target.
    /// </summary>
    public class SeekState : BaseState, INavigationState, IActorDetectionState
    {
        public const ActionState ActionStateValue = ActionState.Seek;

        private const float SeekRadius = 150;
        private const float IdleDuration = 10;

        private Vector2 _seekPosition;

        private float _idleTimer = IdleDuration;
        private bool _isMoving = true;

        public event Action<ActorTag2D> CompleteState;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeekState"/> class.
        /// </summary>
        /// <param name="owner">The owner of the state.</param>
        public SeekState(NPCAgent2D owner, ActionType action) : base(owner, action) { }

        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} SeekState Enter");

            _seekPosition = CommonUtils.GetRandomPosInCircularArea(
                _owner.Parent.GlobalPosition,
                SeekRadius
            );

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
            if (_isMoving) return;

            _idleTimer -= (float)delta;

            if (_idleTimer <= 0)
            {
                GD.Print($"{_owner.Parent.Name} seeking new position");
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
            GD.Print($"{_owner.Parent.Name} SeekState Exit");

            Error result = _owner.EmitSignal(
                NPCAgent2D.SignalName.ActionStateExited,
                Variant.From(ActionStateValue),
                new Array<Variant>()
            );
            DebugTool.Assert(result != Error.Unavailable, "Signal emitted error");
        }

        public bool IsNavigating()
        {
            return _isMoving;
        }

        public Vector2 GetTargetPosition()
        {
            return _seekPosition;
        }

        public bool OnNavigationComplete()
        {
            _isMoving = false;
            return true;
        }

        public void OnActorDetected(ActorTag2D actor)
        {
            if (actor.IsPlayer()) return; // ! Remove after testing
            CompleteState?.Invoke(actor);
        }
    }
}