using NPCProcGen.Core.Components.Enums;
using System;

namespace NPCProcGen.Core.Components
{
    public class ResourceStat
    {
        public ResourceType Type { get; private set; }

        public float Amount
        {
            get => amount;
            set
            {
                amount = Math.Clamp(value, 0, Type == ResourceType.Money ? 1000000 : 100);
                amount = (float)(Type == ResourceType.Money ? Math.Floor(amount) : amount);
            }
        }

        public float Weight { get; private set; }

        public float LowerThreshold { get; private set; }
        public float UpperThreshold { get; private set; }

        private float amount;

        public ResourceStat(ResourceType type, float value, float weight)
        {
            Type = type;
            Amount = value;
            Weight = weight;

            // TODO: Implement thresholds for each resource type
            LowerThreshold = 30;
            UpperThreshold = 80;
        }

        public bool IsImbalanced()
        {
            return IsDeficient() || IsSaturated();
        }

        public bool IsDeficient()
        {
            return amount < LowerThreshold;
        }

        public bool IsSaturated()
        {
            return amount > UpperThreshold;
        }
    }
}