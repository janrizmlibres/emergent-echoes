using System;
using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.States
{
    public class InteractionState : BaseState
    {
        public const ActionState ActionStateValue = ActionState.Interact;

        private readonly ActorTag2D _target;

        public InteractionState(NPCAgent2D owner, ActionType actionType, ActorTag2D target)
            : base(owner, actionType)
        {
            _target = target;
        }

        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} InteractionState Enter");
            _owner.NotifManager.NotifyInteractionStarted();
            _owner.Sensor.SetTaskRecord(_actionType, ActionStateValue);

            Array<Variant> data = new() { _target.Parent };

            Error result = _owner.EmitSignal(
                NPCAgent2D.SignalName.ActionStateEntered,
                Variant.From(ActionStateValue),
                data
            );
            DebugTool.Assert(result != Error.Unavailable, "Signal emitted error");
        }

        public override void Exit()
        {
            _owner.NotifManager.NotifyInteractionEnded();

            Error result = _owner.EmitSignal(
                NPCAgent2D.SignalName.ActionStateExited,
                Variant.From(ActionStateValue),
                new Array<Variant>()
            );
            DebugTool.Assert(result != Error.Unavailable, "Signal emitted error");
        }
    }
}