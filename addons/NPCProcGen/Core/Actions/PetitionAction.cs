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

        private MoveState _moveState;
        private PetitionState _petitionState;

        /// <summary>
        /// Initializes a new instance of the <see cref="PetitionAction"/> class.
        /// </summary>
        /// <param name="owner">The NPC agent performing the action.</param>
        public PetitionAction(NPCAgent2D owner, ActorTag2D target, ResourceType type)
            : base(owner)
        {
            DebugTool.Assert(_owner.Memorizer.GetLastActorLocation(target).HasValue,
                "Target must have a location");

            _target = target;
            _targetResource = type;
            _targetLastPos = _owner.Memorizer.GetLastActorLocation(target).Value;

            InitializeStates();
        }

        private void InitializeStates()
        {
            _moveState = new MoveState(_owner, _target.Parent, _targetLastPos);
            WanderState wanderState = new(_owner, _target);
            _petitionState = new PetitionState(_owner, _target, _targetResource);

            _moveState.CompleteState += (bool isActorDetected) =>
            {
                TransitionTo(isActorDetected ? _petitionState : wanderState);
            };
            wanderState.CompleteState += (bool durationReached) =>
            {
                if (durationReached)
                {
                    CompleteAction(ActionType.Theft);
                }
                else
                {
                    TransitionTo(_petitionState);
                }
            };
            _petitionState.CompleteState += () => CompleteAction(ActionType.Petition);
        }

        public override void Update(double delta)
        {
            _currentState?.Update(delta);
        }

        public override void Run()
        {
            _owner.EmitSignal(NPCAgent2D.SignalName.ExecutionStarted, Variant.From(ActionType.Petition));
            GD.Print($"Petitioning {_target.Parent.Name} for {_targetResource}");
            TransitionTo(_owner.IsActorInRange(_target) ? _petitionState : _moveState);
        }
    }
}