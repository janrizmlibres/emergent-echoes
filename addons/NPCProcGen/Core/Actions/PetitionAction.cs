using Godot;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    /// <summary>
    /// Represents an action where an NPC agent attempts to petition a resource.
    /// </summary>
    public class PetitionAction : BaseAction
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
        private EngageState _engageState;

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

            WanderState wanderState = new(_owner, ActionTypeValue, _targetActor);

            if (targetLastPosition != null)
            {
                _searchState = new(_owner, ActionTypeValue, _targetActor, targetLastPosition.Value);
                _searchState.CompleteState += (bool isTargetFound) =>
                {
                    TransitionTo(isTargetFound ? _engageState : wanderState);
                };
            }
            _engageState = new(_owner, ActionTypeValue, _targetActor, Waypoint.Lateral);


            WaitState waitState = new(_owner, ActionTypeValue, _targetActor);
            PetitionState petitionState = new(_owner, ActionTypeValue, _targetActor, _targetResource);

            wanderState.CompleteState += (bool durationReached) =>
            {
                if (durationReached)
                {
                    CompleteAction();
                }
                else
                {
                    TransitionTo(_targetActor.Sensor.IsActorBusy() ? waitState : _engageState);
                }
            };
            _engageState.CompleteState += (bool isTargetBusy) =>
            {
                TransitionTo(isTargetBusy ? waitState : petitionState);
            };
            waitState.CompleteState += () => TransitionTo(_engageState);
            petitionState.CompleteState += () => CompleteAction();
        }

        public override void Update(double delta)
        {
            _currentState?.Update(delta);
        }

        public override void Run()
        {
            _owner.Sensor.SetPetitionResourceType(_targetResource);

            CommonUtils.EmitSignal(
                _owner,
                NPCAgent2D.SignalName.ExecutionStarted,
                Variant.From(ActionTypeValue)
            );

            if (_owner.IsActorInRange(_targetActor))
            {
                TransitionTo(_engageState);
            }
            else
            {
                DebugTool.Assert(_searchState != null, "Search state is not initialized.");
                TransitionTo(_searchState);
            }
        }
    }
}