using NPCProcGen.Core.Actions;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Traits;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NPCProcGen.Core.Internal
{
    /// <summary>
    /// The Strategizer class is responsible for evaluating and selecting actions based on traits.
    /// </summary>
    public class Strategizer
    {
        /// <summary>
        /// List of traits associated with the strategizer.
        /// </summary>
        private readonly List<Trait> _traits = new();

        /// <summary>
        /// Adds a trait to the strategizer.
        /// </summary>
        /// <param name="trait">The trait to be added.</param>
        public void InitializeTraits(List<Trait> traits)
        {
            _traits.AddRange(traits);
        }

        /// <summary>
        /// Evaluates and selects the best action based on the given social practice.
        /// </summary>
        /// <param name="practice">The social practice to evaluate actions for.</param>
        /// <returns>The best action based on the traits, or null if no action is suitable.</returns>
        public BaseAction EvaluateAction(SocialPractice practice)
        {
            IEnumerable<Tuple<BaseAction, float>> actions = _traits
                .Select(trait => trait.EvaluateAction(practice))
                .Where(action => action != null)
                .OrderByDescending(action => action.Item2);

            return actions.Any() ? actions.First().Item1 : null;
        }

        public BaseAction EvaluateActionStub(Type traitType, Type actionType, ResourceType resType)
        {
            foreach (Trait trait in _traits)
            {
                if (trait.GetType() == traitType)
                {
                    return trait.EvaluateActionStub(actionType, resType);
                }
            }
            return null;
        }
    }
}