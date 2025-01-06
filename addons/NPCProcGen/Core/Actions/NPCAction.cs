using NPCProcGen.Core.States;
using System;

namespace NPCProcGen.Core.Actions
{
    public abstract class NPCAction
    {
        protected readonly ActorTag2D _owner;
        protected ActionState _currentState;

        public NPCAction(ActorTag2D owner)
        {
            _owner = owner;
        }

        protected void TransitionTo(ActionState newState)
        {
            _currentState = newState;
        }

        protected void CompleteAction()
        {
            OnComplete?.Invoke();
        }

        public event Action OnComplete;

        public abstract void Update();
    }
}