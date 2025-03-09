using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.States
{
    public class FindTileState : BaseState, INavigationState
    {
        private readonly CropMarker2D _cropMarker;

        public FindTileState(ActorContext actorContext, StateContext stateContext,
            CropMarker2D cropMarker) : base(actorContext, stateContext, ActionState.FindTile)
        {
            _cropMarker = cropMarker;
        }

        protected override EnterParameters GetEnterData()
        {
            return new EnterParameters
            {
                StateName = "FindTileState",
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

        protected override void ExecuteEnter()
        {
            _cropMarker.Status = CropStatus.Attended;
        }

        public Vector2 GetTargetPosition()
        {
            return _cropMarker.GlobalPosition;
        }

        public bool IsNavigating()
        {
            return true;
        }

        public bool OnNavigationComplete()
        {
            StateContext.Action.TransitionTo(StateContext.ContactState);
            return true;
        }
    }
}