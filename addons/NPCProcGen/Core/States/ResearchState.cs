using System;
using System.Linq;
using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Traits;

namespace NPCProcGen.Core.States
{
    public class ResearchState : BaseState
    {
        public const ActionState ActionStateValue = ActionState.Research;

        private const int MinDuration = 5;
        private const int MaxDuration = 10;

        private readonly bool _isIndeterminate;
        private float _duration;

        public event Action CompleteState;

        public ResearchState(NPCAgent2D owner, ActionType action, bool isIndeterminate = false)
            : base(owner, action)
        {
            _duration = GD.RandRange(MinDuration, MaxDuration);
            _isIndeterminate = isIndeterminate;
        }

        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} ResearchState Enter");
            _owner.Sensor.SetTaskRecord(ActionType.Investigate, ActionState.Research);

            Error result = _owner.EmitSignal(
                NPCAgent2D.SignalName.ActionStateEntered,
                Variant.From(ActionStateValue),
                new Array<Variant>()
            );
            DebugTool.Assert(result != Error.Unavailable, "Signal emitted error");
        }

        public override void Update(double delta)
        {
            _duration -= (float)delta;

            if (_duration <= 0)
            {
                CompleteState?.Invoke();
            }
        }

        public override void Exit()
        {
            if (_isIndeterminate)
            {
                _owner.Traits.OfType<LawfulTrait>().FirstOrDefault()?.MarkCrimeAsUnsolved();
            }

            Array<Variant> data = new() { _isIndeterminate };

            Error result = _owner.EmitSignal(
                NPCAgent2D.SignalName.ActionStateExited,
                Variant.From(ActionStateValue),
                data
            );
            DebugTool.Assert(result != Error.Unavailable, "Signal emitted error");
        }
    }
}