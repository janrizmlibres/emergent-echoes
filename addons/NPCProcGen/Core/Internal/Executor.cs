using System;
using System.Collections.Generic;
using Godot;
using NPCProcGen.Core.Actions;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.Internal
{
    /// <summary>
    /// Executes actions for NPCs.
    /// </summary>
    public class Executor
    {
        private readonly Stack<BaseAction> _actions = new();

        private readonly NPCAgent2D _owner;

        /// <summary>
        /// Event triggered when the execution of an action ends.
        /// </summary>
        public event Action ExecutionEnded;

        public Executor(NPCAgent2D owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// Updates the current action.
        /// </summary>
        /// <param name="delta">The time elapsed since the last update.</param>
        public void Update(double delta)
        {
            if (_actions.TryPeek(out BaseAction action))
            {
                action.Update(delta);
            }
        }

        /// <summary>
        /// Sets a new action to be executed.
        /// </summary>
        /// <param name="action">The action to be executed.</param>
        public void SetAction(BaseAction action)
        {
            DebugTool.Assert(_actions.Count == 0, "Actions should be null when assigning a new action");
            DebugTool.Assert(action != null, "Action to be assigned cannot be null");

            _actions.Push(action);
            action.ActionComplete += OnActionComplete;
            action.Run();
        }

        /// <summary>
        /// Gets the target position of the current action.
        /// </summary>
        /// <returns>The target position.</returns>
        public Vector2 GetTargetPosition()
        {
            if (_actions.TryPeek(out BaseAction action))
            {
                return action.GetTargetPosition();
            }
            else
            {
                return _owner.Parent.GlobalPosition;
            }
        }

        /// <summary>
        /// Checks if there is an action currently being executed.
        /// </summary>
        /// <returns>True if an action is being executed, otherwise false.</returns>
        public bool HasAction()
        {
            return _actions.Count > 0;
        }

        /// <summary>
        /// Queries if the current action involves navigation.
        /// </summary>
        /// <returns>True if the action involves navigation, otherwise false.</returns>
        public bool QueryNavigationAction()
        {
            if (_actions.TryPeek(out BaseAction action))
            {
                return action.IsNavigating();
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Handles the completion of the current action.
        /// </summary>
        private void OnActionComplete()
        {
            DebugTool.Assert(_actions.Count > 0, "Actions cannot be null when completing an action");
            _actions.Pop().ActionComplete -= OnActionComplete;
            ExecutionEnded?.Invoke();
        }
    }
}