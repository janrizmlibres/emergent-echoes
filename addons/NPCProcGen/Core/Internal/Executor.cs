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
        public void AddAction(BaseAction action)
        {
            DebugTool.Assert(action != null, "Action to be assigned cannot be null");

            _owner.Sensor.ClearPetitionResourceType();

            if (_actions.TryPeek(out BaseAction currentAction))
            {
                currentAction.ActionComplete -= OnActionComplete;
                (action as IInteractionAction)?.Unsubscribe();
                currentAction.ClearState();
            }

            _actions.Push(action);
            action.ActionComplete += OnActionComplete;
            (action as IInteractionAction)?.Subscribe();
            action.Run();
        }

        public void EndAllActions()
        {
            while (_actions.Count > 0)
            {
                BaseAction action = _actions.Pop();
                action.ActionComplete -= OnActionComplete;
                (action as IInteractionAction)?.Unsubscribe();
                action.ClearState();
            }

            _owner.Sensor.ClearPetitionResourceType();
            _owner.Sensor.ClearTaskRecord();
        }

        public void EndCurrentAction()
        {
            if (_actions.TryPeek(out BaseAction action))
            {
                action.CompleteAction();
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
        /// Gets the target position of the current action.
        /// </summary>
        /// <returns>The target position.</returns>
        public Vector2 GetTargetPosition()
        {
            if (_actions.TryPeek(out BaseAction action))
            {
                return action.GetTargetPosition();
            }

            return _owner.Parent.GlobalPosition;
        }

        /// <summary>
        /// Queries if the current action involves navigation.
        /// </summary>
        /// <returns>True if the action involves navigation, otherwise false.</returns>
        public bool IsNavigationRequired()
        {
            if (_actions.TryPeek(out BaseAction action))
            {
                return action.IsNavigating();
            }

            return false;
        }

        public bool CompleteNavigation()
        {
            if (_actions.TryPeek(out BaseAction action))
            {
                return action.CompleteNavigation();
            }
            return false;
        }

        public void CompleteConsumption()
        {
            if (_actions.TryPeek(out BaseAction action))
            {
                action.CompleteConsumption();
            }
        }

        public void OnActorDetected(ActorTag2D actor)
        {
            if (_actions.TryPeek(out BaseAction action))
            {
                action.OnActorDetected(actor);
            }
        }

        /// <summary>
        /// Handles the completion of the current action.
        /// </summary>
        private void OnActionComplete()
        {
            DebugTool.Assert(_actions.Count > 0, "Actions cannot be null when completing an action");

            _owner.Sensor.ClearPetitionResourceType();
            _owner.Sensor.ClearTaskRecord();

            BaseAction removedAction = _actions.Pop();
            removedAction.ActionComplete -= OnActionComplete;
            (removedAction as IInteractionAction)?.Unsubscribe();

            if (_actions.TryPeek(out BaseAction action))
            {
                action.ActionComplete += OnActionComplete;
                (action as IInteractionAction)?.Subscribe();
                action.Run();
            }
            else
            {
                ExecutionEnded?.Invoke();
            }
        }
    }
}