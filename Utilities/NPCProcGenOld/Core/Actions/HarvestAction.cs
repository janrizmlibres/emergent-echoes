using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    public class HarvestAction : BaseAction
    {
        private float _harvestTimer = 60;

        public HarvestAction(ActorContext context) : base(context, ActionType.Harvest)
        { }

        protected override void InitializeStates()
        {
            SetupNextHarvest();
        }

        protected override void ExecuteUpdate(double delta)
        {
            _harvestTimer -= (float)delta;
        }

        private bool SetupNextHarvest()
        {
            CropMarker2D cropMarker = ActorContext.Sensor.GetMatureCropTile();

            if (_harvestTimer <= 0 || cropMarker == null)
            {
                ActorContext.Executor.FinishAction();
                return false;
            }

            StateContext.StartingState = new MoveState(
                ActorContext,
                StateContext,
                cropMarker.GlobalPosition
            )
            {
                OnComplete = () => StateContext.Action.TransitionTo(StateContext.ContactState)
            };
            StateContext.ContactState = new HarvestState(
                ActorContext,
                StateContext,
                cropMarker
            )
            {
                OnComplete = () => SetupNextHarvest()
            };

            return true;
        }
    }
}