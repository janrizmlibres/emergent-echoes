using Godot;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    /// <summary>
    /// Represents an action where an NPC agent attempts to petition a resource.
    /// </summary>
    public class PetitionAction : BaseAction, IInteractionAction
    {
        public const ActionType ActionTypeValue = ActionType.Petition;

        /// <summary>
        /// The target actor from which to steal.
        /// </summary>
        private readonly ActorTag2D _targetActor;

        /// <summary>
        /// The type of resource to steal.
        /// </summary>
        private readonly ResourceType _targetResource;

        private SearchState _searchState;
        private WanderState _wanderState;
        private EngageState _engageState;
        private WaitState _waitState;

        /// <summary>
        /// Initializes a new instance of the <see cref="PetitionAction"/> class.
        /// </summary>
        /// <param name="owner">The NPC agent performing the action.</param>
        public PetitionAction(NPCAgent2D owner, ActorTag2D target, ResourceType type)
            : base(owner)
        {
            _targetActor = target;
            _targetResource = type;

            InitializeStates();
        }

        private void InitializeStates()
        {
            Vector2? targetLastPosition = _owner.Memorizer.GetLastKnownPosition(_targetActor);

            _wanderState = new(_owner, ActionTypeValue, _targetActor);

            if (targetLastPosition != null)
            {
                _searchState = new(_owner, ActionTypeValue, _targetActor, targetLastPosition.Value);
                _searchState.CompleteState += isTargetFound =>
                {
                    if (isTargetFound)
                    {
                        TransitionTo(_targetActor.Sensor.IsActorBusy() ? _waitState : _engageState);
                    }
                    else
                    {
                        TransitionTo(_wanderState);
                    }
                };
            }
            _engageState = new(_owner, ActionTypeValue, _targetActor, Waypoint.Lateral);

            _waitState = new(_owner, ActionTypeValue, _targetActor);
            PetitionState petitionState = new(_owner, ActionTypeValue, _targetActor, _targetResource);

            _wanderState.CompleteState += durationReached =>
            {
                if (durationReached)
                {
                    CompleteAction();
                }
                else
                {
                    TransitionTo(_targetActor.Sensor.IsActorBusy() ? _waitState : _engageState);
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
                        TransitionTo(petitionState);
                        break;
                }
            };
            _waitState.CompleteState += () => TransitionTo(_engageState);
            petitionState.CompleteState += () => CompleteAction();
        }

        public void Subscribe()
        {
            _targetActor.NotifManager.ActorImprisoned += InterruptAction;
        }

        public void Unsubscribe()
        {
            _targetActor.NotifManager.ActorImprisoned -= InterruptAction;
        }

        public override void Update(double delta)
        {
            _currentState?.Update(delta);
        }

        public override void Run()
        {
            _owner.Sensor.SetPetitionResourceType(_targetResource);

            Error result = _owner.EmitSignal(
                NPCAgent2D.SignalName.ExecutionStarted,
                Variant.From(ActionTypeValue)
            );
            DebugTool.Assert(result != Error.Unavailable, "Signal emitted error");

            if (_owner.IsActorInRange(_targetActor))
            {
                TransitionTo(_targetActor.Sensor.IsActorBusy() ? _waitState : _engageState);
                return;
            }

            if (_searchState == null)
            {
                Vector2? targetLastPosition = _owner.Memorizer.GetLastKnownPosition(_targetActor);

                if (targetLastPosition == null)
                {
                    CompleteAction();
                    return;
                }

                _searchState = new(_owner, ActionTypeValue, _targetActor, targetLastPosition.Value);
                _searchState.CompleteState += isTargetFound =>
                {
                    if (isTargetFound)
                    {
                        TransitionTo(_targetActor.Sensor.IsActorBusy() ? _waitState : _engageState);
                    }
                    else
                    {
                        TransitionTo(_wanderState);
                    }
                };
            }

            TransitionTo(_searchState);
        }
    }
}