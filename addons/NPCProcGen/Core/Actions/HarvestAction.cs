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
            CropMarker2D cropMarker = _actorContext.Sensor.GetMatureCropTile();

            if (_harvestTimer <= 0 || cropMarker == null)
            {
                _actorContext.Executor.FinishAction();
                return false;
            }

            _stateContext.StartingState = new MoveState(
                _actorContext,
                _stateContext,
                cropMarker.Position
            )
            {
                OnComplete = () => _stateContext.Action.TransitionTo(_stateContext.ContactState)
            };
            _stateContext.ContactState = new HarvestState(
                _actorContext,
                _stateContext,
                cropMarker
            )
            {
                OnComplete = () => SetupNextHarvest()
            };

            return true;
        }
    }
}