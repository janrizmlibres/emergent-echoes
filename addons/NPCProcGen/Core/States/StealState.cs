using Godot;
using NPCProcGen.Core.Actions;

namespace NPCProcGen.Core.States
{
    public class StealState : ActionState
    {
        private readonly ActorTag2D _target;

        public StealState(NPCAgent2D owner, ActorTag2D target) : base(owner)
        {
            _target = target;
        }

        public override void CompleteState()
        {
            throw new System.NotImplementedException();
        }


        public override void Enter()
        {
            GD.Print("StealState Enter");
        }

        public override Vector2 GetTargetPosition()
        {
            return _owner.Parent.GlobalPosition;
        }

    }
}