using System;
using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.States
{
    public class PlantState : BaseState
    {
        private readonly CropMarker2D _cropMarker;

        public Action OnComplete { get; set; }

        public PlantState(ActorContext actorContext, StateContext stateContext,
            CropMarker2D cropMarker)
            : base(actorContext, stateContext, ActionState.Plant)
        {
            _cropMarker = cropMarker;
        }

        protected override EnterParameters GetEnterData()
        {
            return new EnterParameters
            {
                StateName = "PlantState",
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

        public bool OnPlantingComplete()
        {
            _cropMarker.Status = CropStatus.Growing;

            OnComplete?.Invoke();
            _stateContext.Action.TransitionTo(_stateContext.StartingState);
            return true;
        }
    }
}