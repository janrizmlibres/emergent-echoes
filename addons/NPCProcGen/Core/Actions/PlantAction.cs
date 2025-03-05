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

        private bool SetupNextPlanting()
        {
            CropMarker2D cropMarker = ActorContext.Sensor.GetAvailableCropTile();

            if (_plantTimer <= 0 || cropMarker == null)
            {
                ActorContext.Executor.FinishAction();
                return false;
            }

            StateContext.StartingState = new FindTileState(
                ActorContext,
                StateContext,
                cropMarker
            );
            StateContext.ContactState = new PlantState(
                ActorContext,
                StateContext,
                cropMarker
            )
            {
                OnComplete = () => SetupNextPlanting()
            };

            return true;
        }
    }
}