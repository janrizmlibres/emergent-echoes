using System;
using NPCProcGen.Core.Actions;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.Traits
{
    /// <summary>
    /// Represents a trait of an NPC.
    /// </summary>
    public abstract class Trait
    {
        protected readonly NPCAgent2D _owner;
        protected readonly float _weight;
        protected readonly Sensor _sensor;
        protected readonly Memorizer _memorizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Trait"/> class.
        /// </summary>
        /// <param name="owner">The owner of the trait.</param>
        /// <param name="weight">The weight of the trait.</param>
        /// <param name="sensor">The sensor associated with the trait.</param>
        /// <param name="memorizer">The memorizer associated with the trait.</param>
        public Trait(NPCAgent2D owner, float weight, Sensor sensor, Memorizer memorizer)
        {
            _owner = owner;
            _weight = weight;
            _sensor = sensor;
            _memorizer = memorizer;
        }

        /// <summary>
        /// Evaluates an action based on the given social practice.
        /// </summary>
        /// <param name="practice">The social practice to evaluate.</param>
        /// <returns>A tuple containing the evaluated action and its weight.</returns>
        public abstract Tuple<BaseAction, float> EvaluateAction(SocialPractice practice);
    }
}