using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.Traits
{
    public class FarmerTrait : Trait
    {
        public FarmerTrait(ActorContext actorCtx, float weight) : base(actorCtx, weight) { }

        protected override void EvaluateProactiveAction()
        {
            if (Sensor.HasMatureCropTile())
            {
                AddAction(ActionType.Harvest, ResourceType.TotalFood);
                return;
            }

            if (Sensor.HasAvailableCropTile())
            {
                AddAction(ActionType.Plant, ResourceType.TotalFood);
            }
        }
    }
}