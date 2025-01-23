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
        private static readonly Dictionary<ResourceType, bool> _isInteger = new()
        {
            { ResourceType.Money, true },
            { ResourceType.Satiation, true },
            { ResourceType.Companionship, false }
        };

        /// <summary>
        /// Gets the type of resource.
        /// </summary>
        public ResourceType Type { get; private set; }

        /// <summary>
        /// Gets or sets the amount of resource.
        /// </summary>
        public float Amount
        {
            get => amount;
            set
            {
                amount = Math.Clamp(value, 0, Type == ResourceType.Money ? 1000000 : 100);
                amount = (float)(_isInteger[Type] ? Math.Floor(amount) : amount);
            }
        }

        /// <summary>
        /// Gets the weight of the resource.
        /// </summary>
        public float Weight { get; private set; }

        /// <summary>
        /// Gets the lower threshold for the resource amount.
        /// </summary>
        public float LowerThreshold { get; private set; }

        /// <summary>
        /// Gets the upper threshold for the resource amount.
        /// </summary>
        public float UpperThreshold { get; private set; }

        private float amount;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceStat"/> class.
        /// </summary>
        /// <param name="type">The type of resource.</param>
        /// <param name="value">The initial amount of resource.</param>
        /// <param name="weight">The weight of the resource.</param>
        public ResourceStat(ResourceType type, float value, float weight)
        {
            Type = type;
            Amount = value;
            Weight = weight;

            // TODO: Implement thresholds for each resource type
            LowerThreshold = 30;
            UpperThreshold = 80;
        }

        /// <summary>
        /// Determines whether the resource amount is imbalanced.
        /// </summary>
        /// <returns><c>true</c> if the resource amount is imbalanced; otherwise, <c>false</c>.</returns>
        public bool IsImbalanced()
        {
            return IsDeficient() || IsSaturated();
        }

        /// <summary>
        /// Determines whether the resource amount is deficient.
        /// </summary>
        /// <returns><c>true</c> if the resource amount is deficient; otherwise, <c>false</c>.</returns>
        public bool IsDeficient()
        {
            return amount < LowerThreshold;
        }

        /// <summary>
        /// Determines whether the resource amount is saturated.
        /// </summary>
        /// <returns><c>true</c> if the resource amount is saturated; otherwise, <c>false</c>.</returns>
        public bool IsSaturated()
        {
            return amount > UpperThreshold;
        }
    }
}