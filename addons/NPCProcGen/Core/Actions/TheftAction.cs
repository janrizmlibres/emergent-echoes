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

        public TheftAction(NPCAgent2D owner, ActorTag2D target, Vector2 lastPos, ResourceType type)
            : base(owner)
        {
            _target = target;
            _targetLastPos = lastPos;
            _resourceType = type;

            InitializeStates();
        }

        private void InitializeStates()
        {
            StealState stealState = new(_owner, _target, _resourceType);
            FleeState fleeState = new(_owner);

            stealState.OnComplete += () => TransitionTo(fleeState);
            fleeState.OnComplete += () => CompleteAction();

            if (_owner.IsActorInRange(_target))
            {
                TransitionTo(stealState);
                return;
            }

            MoveState moveState = new(_owner, _targetLastPos);
            WanderState wanderState = new(_owner);

            moveState.OnComplete += () => TransitionTo(wanderState);
            wanderState.OnComplete += () => TransitionTo(stealState);

            TransitionTo(moveState);
        }

        public override void Update(double delta)
        {
            _currentState.Update(delta);
        }
    }
}