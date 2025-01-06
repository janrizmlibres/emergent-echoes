namespace NPCProcGen.Core.States
{
    public class StealState : ActionState
    {
        public StealState(ActorTag2D owner) : base(owner) { }

        public override void Update()
        {
            CompleteState();
        }
    }
}