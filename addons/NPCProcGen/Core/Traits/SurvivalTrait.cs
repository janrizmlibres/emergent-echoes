using System;
using System.Collections.Generic;
using System.Linq;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Actions;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.Traits
{
    /// <summary>
    /// Represents a trait for survival actions.
    /// </summary>
    public class SurvivalTrait : Trait
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SurvivalTrait"/> class.
        /// </summary>
        /// <param name="owner">The owner of the trait.</param>
        /// <param name="weight">The weight of the trait.</param>
        /// <param name="sensor">The sensor associated with the trait.</param>
        /// <param name="memorizer">The memorizer associated with the trait.</param>
        public SurvivalTrait(NPCAgent2D owner, float weight, Sensor sensor, Memorizer memorizer)
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

        private Tuple<BaseAction, float> EvaluateProactiveAction()
        {
            List<Tuple<BaseAction, float>> actionCandidates = new();

            List<ResourceType> unevaluatedTypes = ResourceManager.Instance.TangibleTypes;
            ResourceType? selectedType = null;

            while (unevaluatedTypes.Count > 0 && selectedType == null)
            {
                selectedType = SelectDeficientResource(unevaluatedTypes);
                DebugTool.Assert(selectedType != null, "Resource type must not be null");

                float weightedScore = CalculateWeight(selectedType.Value);

                bool hasFood = ResourceManager.Instance.HasResource(_owner, ResourceType.Food);

                if (selectedType == ResourceType.Satiation && hasFood)
                {
                    EatAction eatAction = new(_owner);
                    actionCandidates.Add(new(eatAction, weightedScore));

                    ClearSelection(ref selectedType, unevaluatedTypes);
                    continue;
                }

                ActorTag2D chosenActor = ChooseActor(selectedType.Value, actor => _memorizer.IsFriendly(actor));

                if (chosenActor != null)
                {
                    PetitionAction petitionAction = new(_owner, chosenActor, selectedType.Value);
                    actionCandidates.Add(new(petitionAction, weightedScore));
                }

                ClearSelection(ref selectedType, unevaluatedTypes);
            }

            if (ResourceManager.Instance.IsDeficient(_owner, ResourceType.Companionship))
            {
                float weightedScore = CalculateWeight(ResourceType.Companionship);
                SocializeAction socializeAction = new(_owner);
                actionCandidates.Add(new(socializeAction, weightedScore));
            }

            return actionCandidates.OrderByDescending(action => action.Item2).FirstOrDefault();
        }

        private static void ClearSelection(ref ResourceType? selectedType, List<ResourceType> unevaluatedTypes)
        {
            bool result = unevaluatedTypes.Remove(selectedType.Value);
            DebugTool.Assert(result, "Resource type must be removed from unevaluated types");
            selectedType = null;
        }
    }
}