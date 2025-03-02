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

        private void SetupNextHarvest()
        {
            CropMarker2D cropMarker = _actorContext.Sensor.GetMatureCropTile();

            if (_harvestTimer <= 0 || cropMarker == null)
            {
                _actorContext.Executor.FinishAction();
                return;
            }

            _stateContext.StartingState = new MoveState(
                _actorContext,
                _stateContext,
                cropMarker.Position
            );
            _stateContext.ContactState = new HarvestState(
                _actorContext,
                _stateContext,
                cropMarker
            )
            {
                OnComplete = () => SetupNextHarvest()
            };
        }
    }
}