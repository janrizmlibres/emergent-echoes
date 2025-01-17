using System;
using Godot;
using NPCProcGen.Core.Actions;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.Internal
{
    public class Executor
    {
        // TODO: Convert to a stack of actions to handle intercepts in the middle of an action
        private BaseAction _action = null;

        public event Action ExecutionEnded;

        public void Update(double delta)
        {
            _action?.Update(delta);
        }

        public void SetAction(BaseAction action)
        {
            DebugTool.Assert(_action == null, "Action field should be null when assigning a new action");
            DebugTool.Assert(action != null, "Action to be assigned cannot be null");

            _action = action;
            _action.ActionComplete += OnActionComplete;
            _action.Run();
        }

        public Vector2 GetTargetPosition()
        {
            return _action.GetTargetPosition();
        }

        public bool HasAction()
        {
            return _action != null;
        }

        public bool QueryNavigationAction()
        {
            return _action?.IsNavigating() ?? false;
        }

        public Tuple<ResourceType, float> QueryStolenResource()
        {
            return _action?.GetStolenResource() ?? null;
        }

        private void OnActionComplete()
        {
            DebugTool.Assert(_action != null, "Action field cannot be null when completing an action");

            _action.ActionComplete -= OnActionComplete;
            _action = null;
            ExecutionEnded?.Invoke();
        }
    }
}