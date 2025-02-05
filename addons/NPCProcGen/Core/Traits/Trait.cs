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
        protected readonly NPCMemorizer _memorizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Trait"/> class.
        /// </summary>
        /// <param name="owner">The owner of the trait.</param>
        /// <param name="weight">The weight of the trait.</param>
        /// <param name="sensor">The sensor associated with the trait.</param>
        /// <param name="memorizer">The memorizer associated with the trait.</param>
        public Trait(NPCAgent2D owner, float weight, Sensor sensor, NPCMemorizer memorizer)
        {
            _owner = owner;
            _weight = weight;
            _sensor = sensor;
            _memorizer = memorizer;
        }

        protected void EvaluateInteraction(List<Tuple<BaseAction, float>> actionCandidates, ResourceType type,
            Func<ActorTag2D, bool> bondChecker, Func<List<ActorTag2D>, ActorTag2D> alternator,
            Func<ActorTag2D, Func<BaseAction>> actionCreator)
        {
            ActorTag2D chosenActor = ChooseActor(type, bondChecker, alternator);

            if (chosenActor != null)
            {
                AddAction(actionCandidates, type, actionCreator(chosenActor));
            }
        }

        protected void AddAction(List<Tuple<BaseAction, float>> actionCandidates,
            ResourceType selectedType, Func<BaseAction> actionCreator)
        {
            float weightedScore = CalculateWeight(selectedType);
            actionCandidates.Add(new Tuple<BaseAction, float>(actionCreator(), weightedScore));
        }

        /// <summary>
        /// Chooses an actor to steal from.
        /// </summary>
        /// <param name="type">The resource type to steal.</param>
        /// <returns>The chosen actor.</returns>
        private ActorTag2D ChooseActor(ResourceType type, Func<ActorTag2D, bool> bondChecker,
            Func<List<ActorTag2D>, ActorTag2D> alternator)
        {
            List<ActorTag2D> peerActors = CommonUtils.Shuffle(_owner.Memorizer.GetPeerActors());
            List<ActorTag2D> actorsWithLastPositions = new();

            foreach (ActorTag2D actor in peerActors)
            {
                Vector2? actorLastPos = _owner.Memorizer.GetLastKnownPosition(actor);

                if (actorLastPos == null) continue;

                actorsWithLastPositions.Add(actor);

                // TODO: Add check if actor workplace is known
                if (!ResourceManager.Instance.IsDeficient(actor, type) && bondChecker(actor))
                {
                    return actor;
                }
            }

            return alternator(actorsWithLastPositions);
        }

        private float CalculateWeight(ResourceType type)
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

        // ! Remove in production
        public virtual BaseAction EvaluateActionStub(Type actionType, ResourceType resType)
        {
            return null;
        }
    }
}