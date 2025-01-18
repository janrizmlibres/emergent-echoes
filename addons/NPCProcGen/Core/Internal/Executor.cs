using System;
using Godot;
using NPCProcGen.Core.Actions;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.Internal
{
    /// <summary>
    /// Executes actions for NPCs.
    /// </summary>
    public class Executor
    {
        // TODO: Convert to a stack of actions to handle intercepts in the middle of an action
        private BaseAction _action = null;

        /// <summary>
        /// Event triggered when the execution of an action ends.
        /// </summary>
        public event Action ExecutionEnded;

        /// <summary>
        /// Updates the current action.
        /// </summary>
        /// <param name="delta">The time elapsed since the last update.</param>
        public void Update(double delta)
        {
            _action?.Update(delta);
        }

        /// <summary>
        /// Sets a new action to be executed.
        /// </summary>
        /// <param name="action">The action to be executed.</param>
        public void SetAction(BaseAction action)
        {
            DebugTool.Assert(_action == null, "Action field should be null when assigning a new action");
            DebugTool.Assert(action != null, "Action to be assigned cannot be null");

            _action = action;
            _action.ActionComplete += OnActionComplete;
            _action.Run();
        }

        /// <summary>
        /// Gets the target position of the current action.
        /// </summary>
        /// <returns>The target position.</returns>
        public Vector2 GetTargetPosition()
        {
            return _action.GetTargetPosition();
        }

        /// <summary>
        /// Checks if there is an action currently being executed.
        /// </summary>
        /// <returns>True if an action is being executed, otherwise false.</returns>
        public bool HasAction()
        {
            return _action != null;
        }

        /// <summary>
        /// Queries if the current action involves navigation.
        /// </summary>
        /// <returns>True if the action involves navigation, otherwise false.</returns>
        public bool QueryNavigationAction()
        {
            return _action?.IsNavigating() ?? false;
        }

        /// <summary>
        /// Queries the resource stolen by the current action.
        /// </summary>
        /// <returns>A tuple containing the resource type and amount stolen.</returns>
        public Tuple<ResourceType, float> QueryStolenResource()
        {
            return _action?.GetStolenResource() ?? null;
        }

        /// <summary>
        /// Handles the completion of the current action.
        /// </summary>
        private void OnActionComplete()
        {
            DebugTool.Assert(_action != null, "Action field cannot be null when completing an action");

            _action.ActionComplete -= OnActionComplete;
            _action = null;
            ExecutionEnded?.Invoke();
        }
    }
}