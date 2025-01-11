using System;
using Godot;
using NPCProcGen.Core.Actions;

namespace NPCProcGen.Core.Internal
{
    public class Executor
    {
        // TODO: Convert to a stack of actions to handle intercepts in the middle of an action
        private NPCAction _action = null;

        public event Action OnExecutionEnded;

        public void Update(double delta)
        {
            _action?.Update(delta);
        }

        public void SetAction(NPCAction action)
        {
            _action = action;
            _action.OnComplete += OnActionComplete;
        }

        private void OnActionComplete()
        {
            OnExecutionEnded?.Invoke();
            _action = null;
        }

        public Vector2 GetTargetPosition()
        {
            return _action.GetTargetPosition();
        }

        public bool HasAction()
        {
            return _action != null;
        }

        public bool IsNavigationRequired()
        {
            return _action?.HasNavigationState() ?? false;
        }

        public bool CanSteal()
        {
            return _action.CanSteal();
        }

        public void NotifyNavigationState()
        {
            _action?.NotifyNavigationState();
        }

        public void NotifyStealState()
        {
            _action?.NotifyStealState();
        }
    }
}