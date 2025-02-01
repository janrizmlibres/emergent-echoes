using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
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
                selectedType = SelectResourceType(unevaluatedTypes);
                DebugTool.Assert(selectedType != null, "Resource type must not be null");

                ActorTag2D chosenActor = ChooseActor(selectedType.Value);

                if (chosenActor != null)
                {
                    return CreateTheftAction(chosenActor, selectedType.Value);
                }

                bool result = unevaluatedTypes.Remove(selectedType.Value);
                DebugTool.Assert(result, "Resource type must be removed from unevaluated types");
                selectedType = null;
            }

            return null;
        }

        /// <summary>
        /// Selects a resource type to steal.
        /// </summary>
        /// <param name="unevaluatedTypes">The list of unevaluated resource types.</param>
        /// <returns>The selected resource type.</returns>
        private ResourceType SelectResourceType(List<ResourceType> unevaluatedTypes)
        {
            ResourceType? type = null;

            foreach (var resource in unevaluatedTypes)
            {
                if (ResourceManager.Instance.IsDeficient(_owner, resource))
                {
                    type = resource;
                    break;
                }
            }

            // TODO: If random type is chosen, the final weight of the action should be reduced
            return type ?? unevaluatedTypes[CommonUtils.Rnd.Next(unevaluatedTypes.Count)];
        }

        /// <summary>
        /// Chooses an actor to steal from.
        /// </summary>
        /// <param name="type">The resource type to steal.</param>
        /// <returns>The chosen actor.</returns>
        private ActorTag2D ChooseActor(ResourceType type)
        {
            ActorTag2D result = null;

            List<ActorTag2D> otherActors = _sensor.GetActors()
                .Where(actor => actor != _owner)
                .OrderBy(_ => CommonUtils.Rnd.Next())
                .ToList();

            Vector2? actorLastPos = null;

            foreach (ActorTag2D actor in otherActors)
            {
                actorLastPos = _owner.Memorizer.GetActorLocation(actor);

                // TODO: Add check if actor workplace is known
                // TODO: Check also if imbalance is not too severe
                if (actorLastPos != null
                    && ResourceManager.Instance.HasResource(actor, type)
                    && !_memorizer.IsTrusted(actor))
                {
                    result = actor;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Creates a theft action.
        /// </summary>
        /// <param name="chosenActor">The actor to steal from.</param>
        /// <param name="selectedType">The resource type to steal.</param>
        /// <returns>A tuple containing the theft action and its weight.</returns>
        private Tuple<BaseAction, float> CreateTheftAction(ActorTag2D chosenActor, ResourceType selectedType)
        {
            ResourceStat chosenResource = ResourceManager.Instance.GetResource(_owner, selectedType);

            float imbalance = chosenResource.LowerThreshold - chosenResource.Amount;
            float unweightedScore = Math.Max(0, imbalance) / chosenResource.LowerThreshold;
            float weightedScore = unweightedScore * chosenResource.Weight * _weight;

            TheftAction action = new(_owner, chosenActor, selectedType);
            return new Tuple<BaseAction, float>(action, weightedScore);
        }
    }
}