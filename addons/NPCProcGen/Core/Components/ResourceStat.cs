using NPCProcGen.Core.Components.Enums;
using System.Collections.Generic;

namespace NPCProcGen.Core.Components
{
    public class ResourceStat
    {
        public static readonly List<ResourceType> TangibleTypes = new()
        {
            ResourceType.Money,
            ResourceType.Food
        };

        public ResourceType Type { get; private set; }

        public float Value { get; private set; }
        public float Weight { get; private set; }

        public float LowerThreshold { get; private set; }
        public float UpperThreshold { get; private set; }

        public ResourceStat(ResourceType type, float value, float weight)
        {
            Type = type;
            Value = value;
            Weight = weight;

            // TODO: Implement thresholds for each resource type

            LowerThreshold = 0;
            UpperThreshold = 100;
        }

        public bool IsImbalanced()
        {
            return IsDeficient() || IsSaturated();
        }

        public bool IsDeficient()
        {
            return Value < LowerThreshold;
        }

        public bool IsSaturated()
        {
            return Value > UpperThreshold;
        }
    }
}