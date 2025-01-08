using NPCProcGen.Core.Actions;

namespace NPCProcGen.Core.Internal
{
    public class Executor
    {
        // TODO: Convert to a stack of actions to handle intercepts in the middle of an action
        public NPCAction Action { set; get; } = null;

        public void Update()
        {
            Action?.Update();
        }
    }
}