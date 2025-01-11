using System;
using System.Runtime;
using Godot;
using NPCProcGen.Core.Actions;

namespace NPCProcGen.Core.States
{
    public class MoveState : ActionState, ILinearState
    {
        private readonly Node2D _target = null;
        private Vector2 _targetPosition;

        public event Action OnComplete;

        public MoveState(NPCAgent2D owner, Vector2 targetPosition)
            : base(owner)
        {
            _targetPosition = targetPosition;
        }

        public MoveState(NPCAgent2D owner, Node2D target)
            : base(owner)
        {
            _target = target;
            _targetPosition = target.GlobalPosition;
        }

        public override void Enter()
        {
            GD.Print("MoveState Enter");
        }

        public override void CompleteNavigation()
        {
            OnComplete?.Invoke();
        }

        public override void CompleteState()
        {
            OnComplete?.Invoke();
        }

        public override Vector2 GetTargetPosition()
        {
            return _target?.GlobalPosition ?? _targetPosition;
        }
    }
}