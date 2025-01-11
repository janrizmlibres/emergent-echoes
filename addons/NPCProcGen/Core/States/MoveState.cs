using System;
using Godot;
using NPCProcGen.Core.Actions;

namespace NPCProcGen.Core.States
{
    public class MoveState : ActionState, ILinearState
    {
        private readonly Node2D _target;

        public event Action OnComplete;

        public MoveState(NPCAgent2D owner, Node2D target) : base(owner)
        {
            _target = target;
        }

        public override void Enter()
        {
            GD.Print("MoveState Enter");
        }

        public override void CompleteNavigation()
        {
            OnComplete?.Invoke();
        }

        public override Vector2 GetTargetPosition()
        {
            return _target.GlobalPosition;
        }
    }
}