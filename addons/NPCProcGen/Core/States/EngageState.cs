using System;
using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.States
{
    public enum Waypoint
    {
        Lateral,
        Omni
    }

    public class EngageState : BaseState, INavigationState
    {
        private readonly ActorTag2D _target;
        private readonly Waypoint _waypoint;
        private readonly bool _isInvasive;

        private bool _isTargetReached = false;
        private float _navigationTimer = 15;

        public EngageState(ActorContext actorContext, StateContext stateContext, ActorTag2D target,
            Waypoint waypoint, bool isInvasive = false)
            : base(actorContext, stateContext, ActionState.Engage)
        {
            _target = target;
            _waypoint = waypoint;
            _isInvasive = isInvasive;
        }

        public override void Subscribe()
        {
            if (!_isInvasive) NotifManager.Instance.InteractionStarted += OnInteractionStarted;
        }

        protected override EnterParameters GetEnterData()
        {
            return new EnterParameters
            {
                StateName = "EngageState",
                Data = new Array<Variant>()
            };
        }

        protected override ExitParameters GetExitData()
        {
            return new ExitParameters
            {
                Data = new Array<Variant>()
            };
        }

        public override void Update(double delta)
        {
            if (!_isTargetReached) return;

            _navigationTimer -= (float)delta;

            if (_navigationTimer <= 0)
            {
                _actorContext.Executor.FinishAction();
            }
        }

        public override void Unsubscribe()
        {
            if (!_isInvasive) NotifManager.Instance.InteractionStarted -= OnInteractionStarted;
        }

        public bool IsNavigating()
        {
            return true;
        }

        public Vector2 GetTargetPosition()
        {
            if (_waypoint == Waypoint.Lateral)
                return _target.GetLateralWaypoint(_actorContext.Actor);

            if (_waypoint == Waypoint.Omni)
                return _target.GetOmniDirectionalWaypoint(_actorContext.Actor);

            throw new InvalidOperationException("Invalid waypoint type.");
        }

        public bool OnNavigationComplete()
        {
            _isTargetReached = true;

            // Get current position and target waypoint
            Vector2 currentPos = _actorContext.ActorNode2D.GlobalPosition;
            Vector2 targetWaypoint = GetTargetPosition();

            // Verify we're actually at the waypoint (within reasonable distance)
            float distanceToWaypoint = currentPos.DistanceTo(targetWaypoint);
            if (distanceToWaypoint < 5) // Adjust threshold as needed
            {
                _stateContext.Action.TransitionTo(_stateContext.ContactState);
                return true;
            }

            return false;
        }

        private void OnInteractionStarted(ActorTag2D target)
        {
            if (target != _target) return;
            _stateContext.Action.TransitionTo(_stateContext.WaitState);
        }

        private void OnActorDetained(ActorTag2D actor)
        {
            if (actor != _target) return;
            _actorContext.Executor.TerminateAction();
        }
    }
}