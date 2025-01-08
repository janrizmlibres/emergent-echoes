using Godot;
using NPCProcGen.Core.Actions;

namespace NPCProcGen.Core.States
{
    public class StealState : ActionState
    {
        private readonly ActorTag2D _target;
        private readonly NPCAction _action;

        public StealState(NPCAction action, NPCAgent2D owner, ActorTag2D target) : base(owner)
        {
            _action = action;
            _target = target;
        }

        public override void Enter()
        {
            GD.Print("StealState Enter");
            _action.MoveToTarget(_target.Parent.GlobalPosition);
            _owner.OnFinishNavigation += CompleteState;
        }
    }
}