using System;
using System.Linq;
using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Traits;

namespace NPCProcGen.Core.States
{
    public class CaptureState : BaseState, INavigationState
    {
        public const ActionState ActionStateValue = ActionState.Capture;

        private readonly ActorTag2D _criminal;
        private Vector2 _targetPosition;

        public event Action CompleteState;

        public CaptureState(NPCAgent2D owner, ActionType actionType, ActorTag2D criminal)
            : base(owner, actionType)
        {
            _criminal = criminal;
            _targetPosition = _owner.Parent.GlobalPosition;
        }

        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} CaptureState Enter");

            PrisonArea2D prisonArea = _owner.Sensor.GetPrisonArea();
            DebugTool.Assert(prisonArea != null, "Prison area not found");
            _targetPosition = prisonArea.GetRandomPoint();

            _criminal.Arrest();
            _criminal.EmitSignal(ActorTag2D.SignalName.EventTriggered, Variant.From(EventType.Captured));

            _owner.Sensor.SetTaskRecord(_actionType, ActionStateValue);

            Array<Variant> data = new() { _criminal };

            Error result = _owner.EmitSignal(
                NPCAgent2D.SignalName.ActionStateEntered,
                Variant.From(ActionStateValue),
                data
            );
            DebugTool.Assert(result != Error.Unavailable, "Signal emitted error");
        }

        public override void Exit()
        {
            _owner.Traits.OfType<LawfulTrait>().FirstOrDefault()?.MarkCrimeAsSolved();

            Array<Variant> data = new() { _criminal };

            Error result = _owner.EmitSignal(
                NPCAgent2D.SignalName.ActionStateExited,
                Variant.From(ActionStateValue),
                data
            );
            DebugTool.Assert(result != Error.Unavailable, "Signal emitted error");

            _criminal.EmitSignal(ActorTag2D.SignalName.EventTriggered, Variant.From(EventType.Released));
        }

        public bool IsNavigating()
        {
            return true;
        }

        public Vector2 GetTargetPosition()
        {
            return _targetPosition;
        }

        public bool OnNavigationComplete()
        {
            CompleteState?.Invoke();
            return true;
        }
    }
}