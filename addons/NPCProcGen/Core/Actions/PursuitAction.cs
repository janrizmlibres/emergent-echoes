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

        private bool _doneResearching = false;

        private ResearchState _researchState;
        private EngageState _engageState;

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
            _engageState = new(_owner, ActionTypeValue, _targetActor, Waypoint.Omni);
            CaptureState captureState = new(_owner, ActionTypeValue, _targetActor);

            _researchState.CompleteState += () =>
            {
                _doneResearching = true;
                TransitionTo(_engageState);
            };
            _engageState.CompleteState += outcome =>
            {
                switch (outcome)
                {
                    case EngageOutcome.DurationExceeded:
                        CompleteAction();
                        break;
                    case EngageOutcome.TargetBusy:
                    case EngageOutcome.TargetAvailable:
                        TransitionTo(captureState);
                        break;
                }
            };
            captureState.CompleteState += () => CompleteAction();
        }

        public override void Run()
        {
            Error result = _owner.EmitSignal(
                NPCAgent2D.SignalName.ExecutionStarted,
                Variant.From(ActionTypeValue)
            );
            DebugTool.Assert(result != Error.Unavailable, "Signal emitted error");

            if (!_doneResearching)
            {
                TransitionTo(_researchState);
                return;
            }

            TransitionTo(_engageState);
        }
    }
}