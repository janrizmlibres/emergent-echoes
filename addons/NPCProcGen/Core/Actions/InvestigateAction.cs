using System;
using Godot;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    public class InvestigateAction : BaseAction
    {
        public const ActionType ActionTypeValue = ActionType.Investigate;

        private readonly Crime _crime;
        private ActorTag2D _targetWitness;

        private bool _doneResearching = false;

        private ResearchState _researchState;
        private SearchState _searchState;
        private WanderState _wanderState;
        private EngageState _engageState;
        private WaitState _waitState;

        public InvestigateAction(NPCAgent2D owner, Crime crime) : base(owner)
        {
            _crime = crime;
            InitializeStates();
        }

        private void InitializeStates()
        {
            _researchState = new(_owner, ActionTypeValue);
            _researchState.CompleteState += () =>
            {
                _doneResearching = true;
                CreateInteractStates();
            };
        }

        private void CreateInteractStates()
        {
            Tuple<ActorTag2D, Vector2> targetWitnessData = _crime.GetRandomWitnessData(_owner);

            if (targetWitnessData == null)
            {
                CompleteAction();
                return;
            }

            _targetWitness = targetWitnessData.Item1;
            Vector2 targetLastPosition = targetWitnessData.Item2;

            _searchState = new(_owner, ActionTypeValue, _targetWitness, targetLastPosition);
            _searchState.CompleteState += isTargetFound =>
            {
                if (isTargetFound)
                {
                    TransitionTo(_targetWitness.Sensor.IsActorBusy() ? _waitState : _engageState);
                }
                else
                {
                    TransitionTo(_wanderState);
                }
            };
            _wanderState = new(_owner, ActionTypeValue, _targetWitness);
            _engageState = new(_owner, ActionTypeValue, _targetWitness, Waypoint.Lateral);
            _waitState = new(_owner, ActionTypeValue, _targetWitness);

            InterrogateState interrogateState = new(_owner, ActionTypeValue, _crime, _targetWitness);

            _wanderState.CompleteState += durationReached =>
            {
                if (durationReached)
                {
                    CompleteAction();
                }
                else
                {
                    TransitionTo(_targetWitness.Sensor.IsActorBusy() ? _waitState : _engageState);
                }
            };
            _engageState.CompleteState += outcome =>
            {
                switch (outcome)
                {
                    case EngageOutcome.DurationExceeded:
                        CompleteAction();
                        break;
                    case EngageOutcome.TargetBusy:
                        TransitionTo(_waitState);
                        break;
                    case EngageOutcome.TargetAvailable:
                        TransitionTo(interrogateState);
                        break;
                }
            };
            _waitState.CompleteState += () => TransitionTo(_engageState);
            interrogateState.CompleteState += () => CompleteAction();
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

            if (!_doneResearching)
            {
                TransitionTo(_researchState);
                return;
            }

            if (_owner.IsActorInRange(_targetWitness))
            {
                TransitionTo(_targetWitness.Sensor.IsActorBusy() ? _waitState : _engageState);
                return;
            }

            Vector2? targetLastPosition = _owner.Memorizer.GetLastKnownPosition(_targetWitness);

            if (targetLastPosition == null)
            {
                CompleteAction();
                return;
            }

            TransitionTo(_searchState);
        }
    }
}