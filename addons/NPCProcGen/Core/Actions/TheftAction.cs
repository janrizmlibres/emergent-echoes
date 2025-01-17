using System.Diagnostics;
using Godot;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    public class TheftAction : NPCAction
    {
        private readonly ActorTag2D _target;
        private readonly ResourceType _targetResource;

        private Vector2 _targetLastPos;
        private StealState stealState;

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

        private void InitializeStates()
        {
            MoveState moveState = new(_owner, _target, _targetLastPos);
            WanderState wanderState = new(_owner, _target);
            stealState = new(_owner, _target, _targetResource);
            FleeState fleeState = new(_owner);

            moveState.CompleteState += () => TransitionTo(wanderState);
            wanderState.CompleteState += () => TransitionTo(stealState);
            stealState.CompleteState += () => TransitionTo(fleeState);
            fleeState.CompleteState += () => OnActionComplete();

            TransitionTo(moveState);
        }

        public override void Update(double delta)
        {
            _currentState.Update(delta);
        }
    }
}