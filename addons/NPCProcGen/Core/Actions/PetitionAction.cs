using Godot;
using Godot.Collections;
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
        private readonly ActorTag2D _target;

        /// <summary>
        /// The type of resource to steal.
        /// </summary>
        private readonly ResourceType _targetResource;

        /// <summary>
        /// The last known position of the target.
        /// </summary>
        private Vector2 _targetLastPos;

        private MoveState _initialMoveState;
        private MoveState _moveState;

        /// <summary>
        /// Initializes a new instance of the <see cref="PetitionAction"/> class.
        /// </summary>
        /// <param name="owner">The NPC agent performing the action.</param>
        public PetitionAction(NPCAgent2D owner, ActorTag2D target, ResourceType type)
            : base(owner)
        {
            DebugTool.Assert(_owner.Memorizer.GetLastKnownPosition(target).HasValue,
                "Target must have a location");

            _target = target;
            _targetResource = type;
            _targetLastPos = _owner.Memorizer.GetLastKnownPosition(target).Value;

            InitializeStates();
        }

        private void InitializeStates()
        {
            _initialMoveState = new MoveState(_owner, ActionTypeValue, _target.Parent, _targetLastPos);
            WanderState wanderState = new(_owner, ActionTypeValue, _target);
            _moveState = new(_owner, ActionTypeValue, _target.Parent);
            PetitionState petitionState = new(_owner, ActionTypeValue, _target, _targetResource);

            _initialMoveState.CompleteState += (bool isTargetFound) =>
            {
                TransitionTo(isTargetFound ? petitionState : wanderState);
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
            _moveState.CompleteState += (_) => TransitionTo(petitionState);
            petitionState.CompleteState += () => CompleteAction();
        }

        public override void Update(double delta)
        {
            _currentState?.Update(delta);
        }

        public override void Run()
        {
            CommonUtils.EmitSignal(
                _owner,
                NPCAgent2D.SignalName.ExecutionStarted,
                Variant.From(ActionTypeValue)
            );
            TransitionTo(_owner.IsActorInRange(_target) ? _moveState : _initialMoveState);
        }
    }
}