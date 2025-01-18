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
        /// <summary>
        /// The target actor from which to steal.
        /// </summary>
        private readonly ActorTag2D _target;

        /// <summary>
        /// The type of resource to steal.
        /// </summary>
        private readonly ResourceType _targetResource;

        /// <summary>
        /// The last known position of the target.
        /// </summary>
        private Vector2 _targetLastPos;

        /// <summary>
        /// The state for moving towards the target.
        /// </summary>
        private MoveState moveState;

        /// <summary>
        /// The state for stealing from the target.
        /// </summary>
        private StealState stealState;

        /// <summary>
        /// Initializes a new instance of the <see cref="TheftAction"/> class.
        /// </summary>
        /// <param name="owner">The NPC agent performing the action.</param>
        /// <param name="target">The target actor from which to steal.</param>
        /// <param name="type">The type of resource to steal.</param>
        public TheftAction(NPCAgent2D owner, ActorTag2D target, ResourceType type)
            : base(owner)
        {
            DebugTool.Assert(_owner.Memorizer.GetActorLocation(target).HasValue,
                "Target must have a location");

            _target = target;
            _targetResource = type;
            _targetLastPos = _owner.Memorizer.GetActorLocation(target).Value;

            InitializeStates();
        }

        /// <summary>
        /// Initializes the states for the theft action.
        /// </summary>
        private void InitializeStates()
        {
            moveState = new(_owner, _target.Parent, _targetLastPos);
            WanderState wanderState = new(_owner, _target);
            stealState = new(_owner, _target, _targetResource);
            FleeState fleeState = new(_owner);

            moveState.CompleteState += (bool isActorDetected) =>
            {
                TransitionTo(isActorDetected ? stealState : wanderState);
            };
            wanderState.CompleteState += (bool durationReached) =>
            {
                if (durationReached)
                {
                    CompleteAction(ActionType.Theft);
                }
                else
                {
                    TransitionTo(stealState);
                }
            };
            stealState.CompleteState += () => TransitionTo(fleeState);
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
            TransitionTo(_owner.IsActorInRange(_target) ? stealState : moveState);
        }
    }
}