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
        public const ActionType ActionTypeValue = ActionType.Theft;

        private readonly ActorTag2D _targetActor;
        private readonly ResourceType _targetResource;

        private MoveState _initialMoveState;
        private MoveState _moveState;

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

            _initialMoveState = new(_owner, ActionTypeValue, _targetActor.Parent, targetLastPosition.Value, true);
            WanderState wanderState = new(_owner, ActionTypeValue, _targetActor);
            _moveState = new(_owner, ActionTypeValue, _targetActor.Parent, isStealing: true);
            StealState stealState = new(_owner, ActionTypeValue, _targetActor, _targetResource);
            FleeState fleeState = new(_owner, ActionTypeValue);

            _initialMoveState.CompleteState += (bool isTargetFound) =>
            {
                TransitionTo(isTargetFound ? _moveState : wanderState);
            };
            wanderState.CompleteState += (bool durationReached) =>
            {
                if (durationReached)
                {
                    CompleteAction();
                }
                else
                {
                    TransitionTo(_moveState);
                }
            };
            _moveState.CompleteState += (_) => TransitionTo(stealState);
            stealState.CompleteState += () => TransitionTo(fleeState);
            fleeState.CompleteState += () => CompleteAction();
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
            CommonUtils.EmitSignal(
                _owner,
                NPCAgent2D.SignalName.ExecutionStarted,
                Variant.From(ActionTypeValue)
            );
            TransitionTo(_owner.IsActorInRange(_targetActor) ? _moveState : _initialMoveState);
        }
    }
}