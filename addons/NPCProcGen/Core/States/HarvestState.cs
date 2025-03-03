using System;
using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.States
{
    public class HarvestState : BaseState
    {
        private readonly CropMarker2D _cropMarker;

        public Action OnComplete { get; set; }

        public HarvestState(ActorContext actorContext, StateContext stateContext,
            CropMarker2D cropMarker) : base(actorContext, stateContext, ActionState.Harvest)
        {
            _cropMarker = cropMarker;
        }

        protected override EnterParameters GetEnterData()
        {
            return new EnterParameters
            {
                StateName = "HarvestState",
                Data = new Array<Variant>() { _cropMarker }
            };
        }

        protected override ExitParameters GetExitData()
        {
            return new ExitParameters
            {
                Data = new Array<Variant>()
            };
        }

        public void OnHarvestComplete()
        {
            _cropMarker.Status = CropStatus.Dormant;

            OnComplete?.Invoke();
            _stateContext.Action.TransitionTo(_stateContext.StartingState);
        }
    }
}