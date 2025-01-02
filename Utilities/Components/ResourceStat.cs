using EmergentEchoes.Utilities.Components.Enums;
using Godot;
using System;

namespace EmergentEchoes.Utilities.Components
{
    public class ResourceStat
    {
        // TODO: Implement flyweight pattern for resource types

        public StatType Type { get; private set; }
        public float Value { get; private set; }
        public float Weight { get; private set; }
        public bool IsTangible { get; private set; }

        public float LowerThreshold { get; private set; }
        public float UpperThreshold { get; private set; }

        public ResourceStat(StatType type, float weight, bool isTangible)
        {
            Type = type;
            Weight = weight;
            IsTangible = isTangible;

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