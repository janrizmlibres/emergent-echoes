using System;
using System.Collections.Generic;
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

        protected void EvaluateInteraction(List<Tuple<BaseAction, float>> actionCandidates,
            ResourceType type, Func<List<ActorTag2D>, ActorTag2D> alternator, ActionType actionType)
        {
            ActorTag2D chosenActor = ChooseActor(type, actionType, alternator);

            if (chosenActor != null)
            {
                AddAction(actionCandidates, actionType, type, chosenActor);
            }
        }

        protected void AddAction(List<Tuple<BaseAction, float>> actionCandidates,
            ActionType actionType, ResourceType selectedType, ActorTag2D chosenActor = null)
        {
            float weightedScore = CalculateWeight(selectedType);
            BaseAction action = CreateAction(actionType, chosenActor, selectedType);
            actionCandidates.Add(new(action, weightedScore));
        }

        protected void AddSimpleAction(List<Tuple<BaseAction, float>> actionCandidates,
            Func<BaseAction> actionCreator, float weight)
        {
            BaseAction action = actionCreator();
            actionCandidates.Add(new(action, weight));
        }

        private BaseAction CreateAction(ActionType actionType, ActorTag2D chosenActor, ResourceType resType)
        {
            if (actionType == ActionType.Theft)
                return new TheftAction(_owner, chosenActor, resType);

            if (actionType == ActionType.Petition)
                return new PetitionAction(_owner, chosenActor, resType);

            if (actionType == ActionType.Eat)
                return new EatAction(_owner);

            if (actionType == ActionType.Socialize)
                return new SocializeAction(_owner);

            throw new ArgumentException("Invalid action type");
        }

        /// <summary>
        /// Chooses an actor to steal from.
        /// </summary>
        /// <param name="type">The resource type to steal.</param>
        /// <returns>The chosen actor.</returns>
        private ActorTag2D ChooseActor(ResourceType type, ActionType actionType,
            Func<List<ActorTag2D>, ActorTag2D> actorPicker)
        {
            List<ActorTag2D> peerActors = CommonUtils.Shuffle(_owner.Memorizer.GetPeerActors());
            List<ActorTag2D> potentialActors = new();

            foreach (ActorTag2D actor in peerActors)
            {
                Vector2? actorLastPos = _owner.Memorizer.GetLastKnownPosition(actor);

                if (actorLastPos == null && !_owner.IsActorInRange(actor)) continue;
                if (actor.IsPlayer() && GD.Randf() > 0.2) continue;

                if (actionType == ActionType.Petition)
                {
                    // Equates to false if the actor is not petitioning (petition resource type is null)
                    // or if the actor is petitioning for a different resource type.
                    if (actor.Sensor.GetPetitionResourceType() == type) continue;
                    if (!_owner.Memorizer.IsValidPetitionTarget(actor, type)) continue;
                }

                ResourceManager resMgr = ResourceManager.Instance;

                if (!resMgr.HasResource(actor, type)) continue;
                potentialActors.Add(actor);
            }

            return actorPicker(potentialActors);
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
        public virtual void Update(double delta) { }

        // ! Remove in production
        public virtual BaseAction EvaluateActionStub(Type actionType, ResourceType resType)
        {
            return null;
        }
    }
}