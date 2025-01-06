using System;

namespace NPCProcGen.Core.States
{
    public abstract class ActionState
    {
        protected readonly ActorTag2D _owner;

        public ActionState(ActorTag2D owner)
        {
            _owner = owner;
        }

        protected void CompleteState()
        {
            OnComplete?.Invoke();
        }

        public event Action OnComplete;

        public abstract void Update();
    }
}