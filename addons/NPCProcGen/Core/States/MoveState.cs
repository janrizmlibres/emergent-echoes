using System;
using Godot;
using NPCProcGen.Core.Actions;

namespace NPCProcGen.Core.States
{
    public class MoveState : ActionState
    {
        private readonly Node2D _target;
        private readonly NPCAction _action;

        public MoveState(NPCAction action, NPCAgent2D owner, Node2D target) : base(owner)
        {
            _action = action;
            _target = target;
        }

        public override void Enter()
        {
            GD.Print("MoveState Enter");
            _action.MoveToTarget(_target.GlobalPosition);
            _owner.OnFinishNavigation += CompleteState;
        }
    }
}