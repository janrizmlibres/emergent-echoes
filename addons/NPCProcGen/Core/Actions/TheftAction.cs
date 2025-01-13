using Godot;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    public class TheftAction : NPCAction
    {
        private readonly ActorTag2D _target;
        private readonly ResourceType _resourceType;

        private Vector2 _targetLastPos;

        public TheftAction(NPCAgent2D owner, ActorTag2D target, ResourceType type)
            : base(owner)
        {
            _target = target;
            _resourceType = type;
            _targetLastPos = _owner.Memorizer.GetActorLocation(_target).Value;

            InitializeStates();
        }

        private void InitializeStates()
        {
            StealState stealState = new(_owner, _target, _resourceType);
            FleeState fleeState = new(_owner);

            stealState.StateComplete += () => TransitionTo(fleeState);
            fleeState.StateComplete += OnActionComplete;

            if (_owner.IsActorInRange(_target))
            {
                TransitionTo(stealState);
                return;
            }

            MoveState moveState = new(_owner, _targetLastPos);
            WanderState wanderState = new(_owner, _target);

            moveState.StateComplete += () => TransitionTo(wanderState);
            wanderState.StateComplete += () => TransitionTo(stealState);

            TransitionTo(moveState);
        }

        public override void Update(double delta)
        {
            _currentState.Update(delta);
        }
    }
}