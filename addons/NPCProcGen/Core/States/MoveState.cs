namespace NPCProcGen.Core.States
{
    public class MoveState : ActionState
    {
        private readonly ActorTag2D _target;

        public MoveState(ActorTag2D owner, ActorTag2D target) : base(owner)
        {
            _target = target;
        }

        public override void Update()
        {
            CompleteState();
        }
    }
}