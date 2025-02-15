using Godot;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    public class PursuitAction : BaseAction
    {
        public const ActionType ActionTypeValue = ActionType.Pursuit;

        private readonly ActorTag2D _targetActor;

        private ResearchState _researchState;

        public PursuitAction(NPCAgent2D owner, ActorTag2D target) : base(owner)
        {
            _targetActor = target;
            InitializeStates();
        }

        public override void Update(double delta)
        {
            _currentState?.Update(delta);
        }

        private void InitializeStates()
        {
            _researchState = new(_owner, ActionTypeValue);

            EngageState engageState = new(_owner, ActionTypeValue, _targetActor, Waypoint.Omni);

            _researchState.CompleteState += () =>
            {

            };
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