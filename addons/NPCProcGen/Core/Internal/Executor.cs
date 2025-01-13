using System;
using Godot;
using NPCProcGen.Core.Actions;

namespace NPCProcGen.Core.Internal
{
    public class Executor
    {
        // TODO: Convert to a stack of actions to handle intercepts in the middle of an action
        private NPCAction _action = null;

        public event Action ExecutionEnded;

        public void Update(double delta)
        {
            _action?.Update(delta);
        }

        public void SetAction(NPCAction action)
        {
            _action = action;
            _action.ActionComplete += OnActionComplete;
        }

        public Vector2 GetTargetPosition()
        {
            return _action.GetTargetPosition();
        }

        public bool HasAction()
        {
            return _action != null;
        }

        public bool QueryNavigationState()
        {
            // TODO: Reimplement logic since it's flawed
            return _action?.HasNavigationState() ?? false;
        }

        public bool QueryStealState()
        {
            return _action?.IsStealing() ?? false;
        }

        private void OnActionComplete()
        {
            ExecutionEnded?.Invoke();
            _action = null;
        }
    }
}