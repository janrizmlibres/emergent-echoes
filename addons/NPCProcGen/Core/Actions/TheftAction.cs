using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    public class TheftAction : NPCAction
    {
        private readonly ActorTag2D _target;

        public TheftAction(ActorTag2D owner, ActorTag2D target) : base(owner)
        {
            _currentState = new StealState(owner);
            _target = target;

            InitializeStates();
        }

        private void InitializeStates()
        {
            MoveState moveState = new(_owner, _target);
            StealState stealState = new(_owner);

            OnComplete += () => TransitionTo(moveState);

            moveState.OnComplete += () => TransitionTo(stealState);

            stealState.OnComplete += () => CompleteAction();
            _currentState = moveState;
        }

        public override void Update()
        {
            _currentState.Update();
        }
    }
}