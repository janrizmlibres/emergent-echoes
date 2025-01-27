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

        protected ResourceType SelectDeficientResource(List<ResourceType> types)
        {
            ResourceType? type = null;

            foreach (ResourceType resource in types)
            {
                if (ResourceManager.Instance.IsDeficient(_owner, resource))
                {
                    type = resource;
                    break;
                }
            }

            // TODO: If random type is chosen, the final weight of the action should be reduced
            return type ?? types[CommonUtils.Rnd.Next(types.Count)];
        }

        /// <summary>
        /// Chooses an actor to steal from.
        /// </summary>
        /// <param name="type">The resource type to steal.</param>
        /// <returns>The chosen actor.</returns>
        protected ActorTag2D ChooseActor(ResourceType type, Func<ActorTag2D, bool> trustCheck)
        {
            ActorTag2D result = null;

            List<ActorTag2D> otherActors = _sensor.GetActors()
                .Where(actor => actor != _owner)
                .OrderBy(_ => CommonUtils.Rnd.Next())
                .ToList();

            Vector2? actorLastPos = null;

            foreach (ActorTag2D actor in otherActors)
            {
                actorLastPos = _owner.Memorizer.GetLastActorLocation(actor);

                // TODO: Add check if actor workplace is known
                // TODO: Check also if imbalance is not too severe
                if (actorLastPos != null
                    && ResourceManager.Instance.HasResource(actor, type)
                    && trustCheck(actor))
                {
                    result = actor;
                    break;
                }
            }

            return result;
        }

        protected float CalculateWeight(ResourceType type)
        {
            ResourceStat chosenResource = ResourceManager.Instance.GetResource(_owner, type);
            float imbalance = chosenResource.LowerThreshold - chosenResource.Amount;
            float unweightedScore = Math.Max(0, imbalance) / chosenResource.LowerThreshold;
            return unweightedScore * chosenResource.Weight * _weight;
        }

        /// <summary>
        /// Evaluates an action based on the given social practice.
        /// </summary>
        /// <param name="practice">The social practice to evaluate.</param>
        /// <returns>A tuple containing the evaluated action and its weight.</returns>
        public abstract Tuple<BaseAction, float> EvaluateAction(SocialPractice practice);
    }
}