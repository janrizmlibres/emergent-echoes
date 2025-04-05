using Godot;
using Godot.Collections;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.States
{
    public class CaptureState : BaseState, INavigationState
    {
        private readonly ActorTag2D _criminal;
        private readonly Crime _crime;

        private Vector2 _prisonLocation;

        public CaptureState(ActorContext actorContext, StateContext stateContext,
            ActorTag2D criminal, Crime crime)
            : base(actorContext, stateContext, ActionState.Capture)
        {
            _criminal = criminal;
            _crime = crime;
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
                Data = new Array<Variant> { CommonUtils.DutyIncrease }
            };
        }

        protected override void ExecuteEnter()
        {
            PrisonMarker2D prisonMarker = Sensor.GetRandomPrison();
            _prisonLocation = prisonMarker.GlobalPosition;
            _criminal.TriggerDetainment(ActorContext.Actor);
        }

        protected override void ExecuteExit()
        {
            ResourceManager.Instance.ModifyResource(
                ResourceType.Companionship,
                CommonUtils.DutyIncrease,
                ActorContext.Actor
            );

            _crime.Status = CrimeStatus.Solved;
            _criminal.TriggerCaptivity(_prisonLocation);
        }

        public bool IsNavigating()
        {
            return true;
        }

        public Vector2 GetTargetPosition()
        {
            return CommonUtils.GetOmnidirectionalWaypoint(
                ActorContext.ActorNode2D.GlobalPosition,
                _prisonLocation
            );
        }

        public bool OnNavigationComplete()
        {
            ActorContext.Executor.FinishAction();
            return true;
        }
    }
}