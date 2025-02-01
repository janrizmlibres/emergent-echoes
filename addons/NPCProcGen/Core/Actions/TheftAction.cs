using Godot;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    /// <summary>
    /// Represents an action where an NPC agent attempts to steal a resource from a target.
    /// </summary>
    public class TheftAction : BaseAction
    {
        private readonly ActorTag2D _targetActor;
        private readonly ResourceType _targetResource;

        private MoveState _initialMoveState;
        private StealState _stealState;

        /// <summary>
        /// Initializes a new instance of the <see cref="TheftAction"/> class.
        /// </summary>
        /// <param name="owner">The NPC agent performing the action.</param>
        /// <param name="target">The target actor from which to steal.</param>
        /// <param name="type">The type of resource to steal.</param>
        public TheftAction(NPCAgent2D owner, ActorTag2D target, ResourceType type)
            : base(owner)
        {
            _targetActor = target;
            _targetResource = type;
            InitializeStates();
        }

        /// <summary>
        /// Initializes the states for the theft action.
        /// </summary>
        private void InitializeStates()
        {
            Vector2? targetLastPosition = _owner.Memorizer.GetLastKnownPosition(_targetActor);
            DebugTool.Assert(targetLastPosition != null, "Target must have a location");

            _initialMoveState = new(_owner, _targetActor.Parent, targetLastPosition.Value, true);
            WanderState wanderState = new(_owner, _targetActor);
            MoveState moveState = new(_owner, _targetActor.Parent, isStealing: true);
            _stealState = new(_owner, _targetActor, _targetResource);
            FleeState fleeState = new(_owner);

            _initialMoveState.CompleteState += (bool isTargetFound) =>
            {
                TransitionTo(isTargetFound ? _stealState : wanderState);
            };
            wanderState.CompleteState += (bool durationReached) =>
            {
                if (durationReached)
                {
                    CompleteAction(ActionType.Theft);
                }
                else
                {
                    TransitionTo(moveState);
                }
            };
            moveState.CompleteState += (_) => TransitionTo(_stealState);
            _stealState.CompleteState += () => TransitionTo(fleeState);
            fleeState.CompleteState += () => CompleteAction(ActionType.Theft);
        }

        /// <summary>
        /// Updates the current state of the action.
        /// </summary>
        /// <param name="delta">The time elapsed since the last update.</param>
        public override void Update(double delta)
        {
            _currentState.Update(delta);
        }

        /// <summary>
        /// Starts the theft action.
        /// </summary>
        public override void Run()
        {
            _owner.EmitSignal(NPCAgent2D.SignalName.ExecutionStarted, Variant.From(ActionType.Theft));
            TransitionTo(_owner.IsActorInRange(_targetActor) ? _stealState : _initialMoveState);
        }
    }
}