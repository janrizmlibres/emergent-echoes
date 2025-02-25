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
    public abstract class Trait
    {
        protected readonly ActorContext _actorCtx;
        protected readonly float _weight;

        public Trait(ActorContext actorCtx, float weight)
        {
            _actorCtx = actorCtx;
            _weight = weight;
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

        private BaseAction CreateAction(ActionType actionType, ActorTag2D chosenActor, ResourceType resType)
        {
            if (actionType == ActionType.Theft)
                return new TheftAction(_actorCtx, chosenActor, resType);

            if (actionType == ActionType.Petition)
                return new PetitionAction(_actorCtx, chosenActor, resType);

            if (actionType == ActionType.Eat)
                return new EatAction(_actorCtx);

            if (actionType == ActionType.Socialize)
                return new SocializeAction(_actorCtx);

            throw new ArgumentException("Invalid action type");
        }

        private ActorTag2D ChooseActor(ResourceType type, ActionType actionType,
            Func<List<ActorTag2D>, ActorTag2D> actorPicker)
        {
            List<ActorTag2D> peerActors = CommonUtils.Shuffle(_actorCtx.Memorizer.GetPeerActors());
            List<ActorTag2D> potentialActors = new();

            foreach (ActorTag2D actor in peerActors)
            {
                if (!actor.IsValidTarget(_actorCtx.GetNPCAgent2D())) continue;

                if (actionType == ActionType.Petition)
                {
                    if (actor.Sensor.GetPetitionResourceType() == type) continue;
                    if (!_actorCtx.Memorizer.IsValidPetitionTarget(actor, type)) continue;
                }

                if (!ResourceManager.Instance.HasResource(actor, type)) continue;
                potentialActors.Add(actor);
            }

            return actorPicker(potentialActors);
        }

        private float CalculateWeight(ResourceType type)
        {
            ResourceStat chosenResource = ResourceManager.Instance.GetResource(_actorCtx.Actor, type);

            float imbalance = chosenResource.LowerThreshold - chosenResource.Amount;
            float unweightedScore = Math.Max(0, imbalance) / chosenResource.LowerThreshold;

            return unweightedScore * chosenResource.Weight * _weight;
        }

        public abstract Tuple<BaseAction, float> EvaluateAction(SocialPractice practice);

        public virtual void Update(double delta) { }

        // ! Remove in production
        public virtual BaseAction EvaluateActionStub(Type actionType, ResourceType resType)
        {
            return null;
        }
    }
}