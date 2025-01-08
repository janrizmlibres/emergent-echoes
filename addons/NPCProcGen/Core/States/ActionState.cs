using System;

namespace NPCProcGen.Core.States
{
    public abstract class ActionState
    {
        protected readonly NPCAgent2D _owner;

        public ActionState(NPCAgent2D owner)
        {
            _owner = owner;
        }

        protected void CompleteState()
        {
            OnComplete?.Invoke();
        }

        public event Action OnComplete;

        public virtual void Enter() { }
        public virtual void Update() { }
    }
}