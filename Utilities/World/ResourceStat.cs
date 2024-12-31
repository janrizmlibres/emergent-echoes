using Godot;
using System;

namespace EmergentEchoes.Utilities.World
{
    public class ResourceStat
    {
        public enum Type
        {
            Money,
            Food,
            Companionship,
            Social
        }

        // TODO: Implement flyweight pattern for resource types
        // TODO: Make dedicated classes for each resource type

        public Type ResourceType { get; private set; }
        public float Value { get; private set; }
        public float Weight { get; private set; }
        public bool IsTangible { get; private set; }

        private readonly float _lowerThreshold;
        private readonly float _upperThreshold;

        public ResourceStat(Type type, float weight, bool isTangible)
        {
            ResourceType = type;
            Weight = weight;
            IsTangible = isTangible;

            _lowerThreshold = 0;
            _upperThreshold = 100;
        }

        public bool IsImbalanced()
        {
            return IsTangible && (Value < _lowerThreshold || Value > _upperThreshold);
        }
    }
}