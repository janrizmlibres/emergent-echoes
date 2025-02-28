using NPCProcGen.Core.Components.Enums;
using System;
using System.Collections.Generic;

namespace NPCProcGen.Core.Components
{
    /// <summary>
    /// Represents the statistics of a resource.
    /// </summary>
    public class ResourceStat
    {
        // * Decay rate per second
        private const float DecayRate = 0.1f;

        private static readonly Dictionary<
            ResourceType,
            (float MinLow, float MaxLow, float MinHigh, float MaxHigh)
        > _thresholdValues = new()
        {
            { ResourceType.Money, (100, 500, 1000, 5000) },
            { ResourceType.Food, (5, 10, 50, 100) },
            { ResourceType.Satiation, (15, 25, 70, 90) },
            { ResourceType.Companionship, (10, 20, 60, 80) },
        };

        public ResourceType Type { get; private set; }

        public float Amount
        {
            get => _amount;
            set
            {
                _amount = Math.Clamp(value, 0, GetMaxValue());
                _amount = (float)(IsInteger() ? Math.Floor(_amount) : _amount);
            }
        }

        public float Weight { get; private set; }

        public int LowerThreshold { get; private set; }

        public int UpperThreshold { get; private set; }

        private float _amount;

        public ResourceStat(ResourceType type, float value, float weight)
        {
            Type = type;
            Amount = value;
            Weight = weight;

            (float MinLow, float MaxLow, float MinHigh, float MaxHigh) = _thresholdValues[type];
            LowerThreshold = (int)(MinLow + Weight * (MaxLow - MinLow));
            UpperThreshold = (int)(MinHigh + Weight * (MaxHigh - MinHigh));
        }

        public void Update(double delta)
        {
            if (CanDecay())
            {
                Amount -= (float)(DecayRate * delta);
            }
        }

        public int GetMinRaise()
        {
            return Type == ResourceType.Money ? 10 : 1;
        }

        private float GetMaxValue()
        {
            return Type switch
            {
                ResourceType.Money => 1000000,
                ResourceType.Food => 1000,
                ResourceType.Satiation => 100,
                ResourceType.Companionship => 100,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        private bool CanDecay()
        {
            return Type == ResourceType.Satiation || Type == ResourceType.Companionship;
        }

        private bool IsInteger()
        {
            return Type == ResourceType.Money || Type == ResourceType.Food;
        }

        /// <summary>
        /// Determines whether the resource amount is imbalanced.
        /// </summary>
        /// <returns><c>true</c> if the resource amount is imbalanced; otherwise, <c>false</c>.</returns>
        public bool IsImbalanced()
        {
            return IsDeficient() || IsSurplus();
        }

        /// <summary>
        /// Determines whether the resource amount is deficient.
        /// </summary>
        /// <returns><c>true</c> if the resource amount is deficient; otherwise, <c>false</c>.</returns>
        public bool IsDeficient()
        {
            return _amount < LowerThreshold;
        }

        /// <summary>
        /// Determines whether the resource amount is saturated.
        /// </summary>
        /// <returns><c>true</c> if the resource amount is saturated; otherwise, <c>false</c>.</returns>
        public bool IsSurplus()
        {
            return _amount > UpperThreshold;
        }
    }
}