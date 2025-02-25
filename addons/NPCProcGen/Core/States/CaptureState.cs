using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.States
{
    public class CaptureState : BaseState, INavigationState
    {
        private readonly ActorTag2D _criminal;
        private Vector2 _prisonLocation;

        public CaptureState(ActorContext actorContext, StateContext stateContext,
            ActorTag2D criminal) : base(actorContext, stateContext, ActionState.Capture)
        {
            _criminal = criminal;
        }

        protected override EnterParameters GetEnterData()
        {
            return new EnterParameters
            {
                StateName = "CaptureState",
                Data = new Array<Variant> { _criminal.GetParent().Name }
            };
        }

        protected override ExitParameters GetExitData()
        {
            return new ExitParameters
            {
                Data = new Array<Variant>()
            };
        }

        protected override void ExecuteEnter()
        {
            PrisonMarker2D prisonMarker = _actorContext.Sensor.GetRandomPrison();
            _prisonLocation = prisonMarker.GlobalPosition;
            _criminal.TriggerDetainment();
        }

        protected override void ExecuteExit()
        {
            _actorContext.LawfulModule.ClearCase(CrimeStatus.Solved);
            _criminal.TriggerCaptivity(_prisonLocation);
        }

        public bool IsNavigating()
        {
            return true;
        }

        public Vector2 GetTargetPosition()
        {
            return CommonUtils.GetOmnidirectionalWaypoint(
                _actorContext.ActorNode2D.GlobalPosition,
                _prisonLocation
            );
        }

        public bool OnNavigationComplete()
        {
            _actorContext.Executor.FinishAction();
            return true;
        }
    }
}