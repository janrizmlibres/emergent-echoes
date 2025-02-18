using Godot;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    public class CloseCaseAction : BaseAction
    {
        public const ActionType ActionTypeValue = ActionType.CloseCase;

        private readonly ResearchState _researchState;

        public CloseCaseAction(NPCAgent2D owner) : base(owner)
        {
            _researchState = new(owner, ActionTypeValue, true);
            _researchState.CompleteState += () => CompleteAction();
        }

        public override void Update(double delta)
        {
            _currentState?.Update(delta);
        }

        public override void Run()
        {
            Error result = _owner.EmitSignal(
                NPCAgent2D.SignalName.ExecutionStarted,
                Variant.From(ActionTypeValue)
            );
            DebugTool.Assert(result != Error.Unavailable, "Signal emitted error");

            TransitionTo(_researchState);
        }
    }
}