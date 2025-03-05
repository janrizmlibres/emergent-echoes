using System;
using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.States
{
    public class SeekState : BaseState, INavigationState, IActorDetectionState
    {
        private const float SeekRadius = 150;
        private const float IdleDuration = 10;

        private readonly Action<ActorTag2D> _setupInteractStates;
        private Vector2 _seekPosition;

        private float _idleTimer = IdleDuration;
        private bool _isMoving = true;

        public SeekState(ActorContext actorContext, StateContext stateContext,
            Action<ActorTag2D> setupInteractStates)
            : base(actorContext, stateContext, ActionState.Seek)
        {
            _setupInteractStates = setupInteractStates;
        }

        protected override bool Validate()
        {
            NPCAgent2D agent = ActorContext.GetNPCAgent2D();

            if (agent.IsAnyActorInRange())
            {
                ActorTag2D target = agent.GetRandomActorInRange();
                _setupInteractStates(target);
                return false;
            }

            return true;
        }

        protected override void ExecuteEnter()
        {
            _seekPosition = CommonUtils.GetRandomPosInCircularArea(
                ActorContext.ActorNode2D.GlobalPosition,
                SeekRadius
            );
        }

        protected override EnterParameters GetEnterData()
        {
            return new EnterParameters
            {
                StateName = "SeekState",
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
            if (_isMoving) return;

            _idleTimer -= (float)delta;

            if (_idleTimer <= 0)
            {
                _seekPosition = CommonUtils.GetRandomPosInCircularArea(
                    ActorContext.ActorNode2D.GlobalPosition,
                    SeekRadius
                );
                _idleTimer = IdleDuration;
                _isMoving = true;
            }
        }

        public bool IsNavigating()
        {
            return _isMoving;
        }

        public Vector2 GetTargetPosition()
        {
            return _seekPosition;
        }

        public bool OnNavigationComplete()
        {
            _isMoving = false;
            return true;
        }

        public void OnActorDetected(ActorTag2D actor)
        {
            if (!actor.CanInteract()) return;
            _setupInteractStates(actor);
        }
    }
}