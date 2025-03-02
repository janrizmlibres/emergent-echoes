using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    public class PlantAction : BaseAction
    {
        private float _plantTimer = 60;

        public PlantAction(ActorContext context) : base(context, ActionType.Plant) { }

        protected override void InitializeStates()
        {
            SetupNextPlanting();
        }

        protected override void ExecuteUpdate(double delta)
        {
            _plantTimer -= (float)delta;
        }

        private void SetupNextPlanting()
        {
            CropMarker2D cropMarker = _actorContext.Sensor.GetAvailableCropTile();

            if (_plantTimer <= 0 || cropMarker == null)
            {
                _actorContext.Executor.FinishAction();
                return;
            }

            _stateContext.StartingState = new FindTileState(
                _actorContext,
                _stateContext,
                cropMarker
            );
            _stateContext.ContactState = new PlantState(
                _actorContext,
                _stateContext,
                cropMarker
            )
            {
                OnComplete = () => SetupNextPlanting()
            };
        }
    }
}