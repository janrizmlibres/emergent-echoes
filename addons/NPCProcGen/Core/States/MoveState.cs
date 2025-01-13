using System;
using Godot;

namespace NPCProcGen.Core.States
{
    public class MoveState : ActionState, ILinearState
    {
        private readonly Node2D _targetNode = null;
        private readonly ActorTag2D _target = null;
        private Vector2 _targetPosition;

        public event Action StateComplete;

        public MoveState(NPCAgent2D owner, Vector2 targetPosition)
            : base(owner)
        {
            _targetPosition = targetPosition;
        }

        public MoveState(NPCAgent2D owner, ActorTag2D target)
            : base(owner)
        {
            _target = target;
            _targetPosition = target.Parent.GlobalPosition;
        }

        public MoveState(NPCAgent2D owner, Node2D target)
            : base(owner)
        {
            _targetNode = target;
            _targetPosition = target.GlobalPosition;
        }

        public override void Enter()
        {
            GD.Print("MoveState Enter");
            _owner.NotifManager.NavigationComplete += OnNavigationComplete;
            _owner.NotifManager.ActorDetected += OnActorDetected;
        }

        public override Vector2 GetTargetPosition()
        {
            return _target?.Parent.GlobalPosition
                ?? _targetNode?.GlobalPosition
                ?? _targetPosition;
        }

        private void OnNavigationComplete()
        {
            StateComplete?.Invoke();
        }

        private void OnActorDetected(ActorTag2D actor)
        {
            if (actor == _target)
            {
                StateComplete?.Invoke();
            }
        }
    }
}