using NPCProcGen.Core.Actions;

namespace NPCProcGen.Core.Internal
{
    public class Executor
    {
        // TODO: Convert to a stack of actions to handle intercepts in the middle of an action
        private NPCAction _action;

        public void SetAction(NPCAction action)
        {
            _action = action;
            _action.OnComplete += () => _action = null;
        }

        public void Update()
        {
            _action?.Update();
        }

        public bool IsExecuting()
        {
            return _action != null;
        }
    }
}