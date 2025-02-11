using System;
using Godot;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.States
{
    public class SearchState : BaseState, INavigationState, IActorDetectionState
    {
        public const ActionState ActionStateValue = ActionState.Search;

        private readonly ActorTag2D _target;
        private Vector2 _lastKnownPosition;

        /// <summary>
        /// Event triggered when the state is completed.
        /// </summary>
        public event Action<bool> CompleteState;

        public SearchState(NPCAgent2D owner, ActionType action, ActorTag2D target,
            Vector2 lastKnownPosition) : base(owner, action)
        {
            _target = target;
            _lastKnownPosition = lastKnownPosition;
        }

        /// <summary>
        /// Called when the state is entered.
        /// </summary>
        public override void Enter()
        {
            // GD.Print($"{_owner.Parent.Name} SearchState Enter");

            _owner.Sensor.SetTaskRecord(_actionType, ActionStateValue);

            CommonUtils.EmitSignal(
                _owner,
                NPCAgent2D.SignalName.ActionStateEntered,
                Variant.From(ActionStateValue)
            );
        }

        /// <summary>
        /// Called when the state is exited.
        /// </summary>
        public override void Exit()
        {
            CommonUtils.EmitSignal(
                _owner,
                NPCAgent2D.SignalName.ActionStateExited,
                Variant.From(ActionStateValue)
            );
        }

        /// <summary>
        /// Determines whether the agent is currently navigating.
        /// </summary>
        /// <returns>True if the agent is navigating; otherwise, false.</returns>
        public bool IsNavigating()
        {
            return true;
        }

        /// <summary>
        /// Gets the target position for navigation.
        /// </summary>
        /// <returns>The global position of the target.</returns>
        public Vector2 GetTargetPosition()
        {
            return _lastKnownPosition;
        }

        public bool OnNavigationComplete()
        {
            bool isTargetFound = false;
            CompleteState?.Invoke(isTargetFound);
            return true;
        }

        public void OnActorDetected(ActorTag2D target)
        {
            if (target == _target)
            {
                bool isTargetFound = true;
                CompleteState?.Invoke(isTargetFound);
            }
        }
    }
}