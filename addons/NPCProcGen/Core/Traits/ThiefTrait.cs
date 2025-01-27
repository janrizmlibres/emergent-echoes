using System;
using System.Collections.Generic;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Actions;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
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
            List<ResourceType> unevaluatedTypes = ResourceManager.Instance.TangibleTypes;
            ResourceType? selectedType = null;

            while (unevaluatedTypes.Count > 0 && selectedType == null)
            {
                selectedType = SelectDeficientResource(unevaluatedTypes);
                DebugTool.Assert(selectedType != null, "Resource type must not be null");

                if (selectedType == ResourceType.Satiation && _owner.FoodValue > 0)
                {
                    ClearSelection(ref selectedType, unevaluatedTypes);
                    continue;
                }

                ActorTag2D chosenActor = ChooseActor(selectedType.Value, actor => !_memorizer.IsTrusted(actor));

                if (chosenActor != null)
                {
                    return CreateTheftAction(chosenActor, selectedType.Value);
                }

                ClearSelection(ref selectedType, unevaluatedTypes);
            }

            return null;
        }

        private static void ClearSelection(ref ResourceType? selectedType, List<ResourceType> unevaluatedTypes)
        {
            bool result = unevaluatedTypes.Remove(selectedType.Value);
            DebugTool.Assert(result, "Resource type must be removed from unevaluated types");
            selectedType = null;
        }

        /// <summary>
        /// Creates a theft action.
        /// </summary>
        /// <param name="chosenActor">The actor to steal from.</param>
        /// <param name="selectedType">The resource type to steal.</param>
        /// <returns>A tuple containing the theft action and its weight.</returns>
        private Tuple<BaseAction, float> CreateTheftAction(ActorTag2D chosenActor, ResourceType selectedType)
        {
            float weightedScore = CalculateWeight(selectedType);
            TheftAction action = new(_owner, chosenActor, selectedType);
            return new Tuple<BaseAction, float>(action, weightedScore);
        }
    }
}