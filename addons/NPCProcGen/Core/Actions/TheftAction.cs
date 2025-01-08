using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    public class TheftAction : NPCAction
    {
        private readonly ActorTag2D _target;

        public TheftAction(NPCAgent2D owner, ActorTag2D target) : base(owner)
        {
            _target = target;

            InitializeStates();
        }

        private void InitializeStates()
        {
            MoveState moveState = new(this, _owner, _target.StealMarker);
            FleeState fleeState = new(this, _owner);

            moveState.OnComplete += () => TransitionTo(fleeState);
            fleeState.OnComplete += () => CompleteAction();

            _currentState = moveState;
        }

        public override void Update()
        {
            _currentState.Update();
        }
    }
}