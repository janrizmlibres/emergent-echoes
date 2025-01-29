using System;
using System.Collections.Generic;
using System.Linq;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Actions;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.Traits
{
    /// <summary>
    /// Represents a trait for stealing resources.
    /// </summary>
    public class ThiefTrait : Trait
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThiefTrait"/> class.
        /// </summary>
        /// <param name="owner">The owner of the trait.</param>
        /// <param name="weight">The weight of the trait.</param>
        /// <param name="sensor">The sensor associated with the trait.</param>
        /// <param name="memorizer">The memorizer associated with the trait.</param>
        public ThiefTrait(NPCAgent2D owner, float weight, Sensor sensor, Memorizer memorizer)
            : base(owner, weight, sensor, memorizer) { }

        /// <summary>
        /// Evaluates an action based on the given social practice.
        /// </summary>
        /// <param name="practice">The social practice to evaluate.</param>
        /// <returns>A tuple containing the evaluated action and its weight.</returns>
        public override Tuple<BaseAction, float> EvaluateAction(SocialPractice practice)
        {
            if (practice == SocialPractice.Proactive)
            {
                return EvaluateProactiveAction();
            }

            return null;
        }

        /// <summary>
        /// Evaluates a proactive action for stealing resources.
        /// </summary>
        /// <returns>A tuple containing the evaluated action and its weight.</returns>
        private Tuple<BaseAction, float> EvaluateProactiveAction()
        {
            ResourceManager resourceMgr = ResourceManager.Instance;
            List<Tuple<BaseAction, float>> actionCandidates = new();

            foreach (ResourceType type in resourceMgr.TangibleTypes)
            {
                if (resourceMgr.IsDeficient(_owner, type))
                {
                    EvaluateInteraction(
                        actionCandidates, type,
                        actor => !_memorizer.IsTrusted(actor),
                        (_) => null,
                        (chosenActor) => () => new TheftAction(_owner, chosenActor, type)
                    );
                }
            }

            return actionCandidates.OrderByDescending(tuple => tuple.Item2).FirstOrDefault();
        }
    }
}