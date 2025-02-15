using System;
using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.States
{
    public class CaptureState : BaseState, INavigationState
    {
        public const ActionState ActionStateValue = ActionState.Capture;

        private readonly Marker2D _jailMarker;

        public event Action CompleteState;

        public CaptureState(NPCAgent2D owner, ActionType actionType, Marker2D marker)
            : base(owner, actionType)
        {
            _jailMarker = marker;
        }

        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} CaptureState Enter");

            _owner.Sensor.SetTaskRecord(_actionType, ActionStateValue);

            // data here

            Error result = _owner.EmitSignal(
                NPCAgent2D.SignalName.ActionStateEntered,
                Variant.From(ActionStateValue),
                new Array<Variant>()
            );
            DebugTool.Assert(result != Error.Unavailable, "Signal emitted error");
        }

        public override void Exit()
        {
            GD.Print($"{_owner.Parent.Name} EatState Exit");

            // Data here

            Error result = _owner.EmitSignal(
                NPCAgent2D.SignalName.ActionStateExited,
                Variant.From(ActionStateValue),
                new Array<Variant>()
            );
            DebugTool.Assert(result != Error.Unavailable, "Signal emitted error");
        }

        public bool IsNavigating()
        {
            return true;
        }

        public Vector2 GetTargetPosition()
        {
            return _jailMarker.GlobalPosition;
        }

        public bool OnNavigationComplete()
        {
            CompleteState?.Invoke();
            return true;
        }
    }
}