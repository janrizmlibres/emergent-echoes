using System.Numerics;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    public enum Rando
    {
        One,
        Two,
        Three
    }

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
            MoveState moveState = new(_owner, _target.StealMarker);
            FleeState fleeState = new(_owner);

            moveState.OnComplete += () => TransitionTo(fleeState);
            fleeState.OnComplete += () => CompleteAction();

            TransitionTo(moveState);
        }

        public override void Update()
        {
            _currentState.Update();
        }
    }
}