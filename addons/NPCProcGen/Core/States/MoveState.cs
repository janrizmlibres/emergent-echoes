using System;
using Godot;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.States
{
    public class MoveState : ActionState, INavigationState
    {
        private readonly Node2D _targetNode = null;
        private readonly ActorTag2D _target = null;
        private Vector2? _targetPosition = null;

        public event Action CompleteState;

        public MoveState(NPCAgent2D owner, Node2D target)
            : base(owner)
        {
            _targetNode = target;
        }

        public MoveState(NPCAgent2D owner, ActorTag2D target, Vector2? lastPos = null)
            : base(owner)
        {
            _target = target;
            _targetPosition = lastPos;
        }

        public MoveState(NPCAgent2D owner, Vector2 targetPosition)
            : base(owner)
        {
            _targetPosition = targetPosition;
        }

        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} MoveState Enter");
            _owner.NotifManager.NavigationComplete += OnNavigationComplete;
        }

        public override void Update(double delta)
        {
            if (_owner.IsActorInRange(_target))
            {
                OnNavigationComplete();
            }
        }

        public bool IsNavigating()
        {
            return true;
        }

        public Vector2 GetTargetPosition()
        {
            if (_targetNode != null)
            {
                return _targetNode.GlobalPosition;
            }

            return _targetPosition ?? _target.Parent.GlobalPosition;
        }

        private void OnNavigationComplete()
        {
            _owner.NotifManager.NavigationComplete -= OnNavigationComplete;
            CompleteState?.Invoke();
        }
    }
}