using Godot;

namespace NPCProcGen.Core.States
{
    public class MoveState : ActionState
    {
        private readonly Node2D _target;

        public MoveState(ActorTag2D owner, Node2D target) : base(owner)
        {
            _target = target;
        }

        public override void Update()
        {
            CompleteState();
        }
    }
}