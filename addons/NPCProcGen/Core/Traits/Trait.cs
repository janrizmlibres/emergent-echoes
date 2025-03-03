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
    public abstract class Trait
    {
        protected readonly ActorContext _actorCtx;
        protected readonly float _weight;

        private readonly List<Tuple<BaseAction, float>> _actionCandidates = new();

        public Trait(ActorContext actorCtx, float weight)
        {
            _actorCtx = actorCtx;
            _weight = weight;
        }

        public Tuple<BaseAction, float> EvaluateAction(SocialPractice practice)
        {
            _actionCandidates.Clear();

            if (practice == SocialPractice.Proactive)
            {
                EvaluateProactiveAction();
            }

            return _actionCandidates
                .OrderByDescending(action => action.Item2)
                .FirstOrDefault();
        }

        protected abstract void EvaluateProactiveAction();

        protected void EvaluateInteraction(ResourceType type, Func<List<ActorTag2D>,
            ActorTag2D> alternator, ActionType actionType)
        {
            ActorTag2D chosenActor = ChooseActor(type, actionType, alternator);
            if (chosenActor == null) return;
            AddAction(actionType, type, chosenActor);

            ActorTag2D ChooseActor(ResourceType type, ActionType actionType,
                Func<List<ActorTag2D>, ActorTag2D> actorPicker)
            {
                List<ActorTag2D> peerActors = CommonUtils.Shuffle(
                    _actorCtx.Memorizer.GetPeerActors()
                );

                List<ActorTag2D> potentialActors = new();

                foreach (ActorTag2D actor in peerActors)
                {
                    if (!actor.IsValidTarget(_actorCtx.GetNPCAgent2D())) continue;

                    if (actionType == ActionType.Petition)
                    {
                        if (actor.Sensor.GetPetitionResourceType() == type) continue;
                        if (!_actorCtx.Memorizer.IsValidPetitionTarget(actor, type)) continue;
                    }

                    if (!ResourceManager.Instance.HasResource(type, actor)) continue;
                    potentialActors.Add(actor);
                }

                return actorPicker(potentialActors);
            }
        }

        protected void AddAction(ActionType actionType, ResourceType type,
            ActorTag2D chosenActor = null, Crime crime = null)
        {
            float weightedScore = CalculateWeight(type);
            BaseAction action = CreateAction(actionType, chosenActor, type, crime);
            _actionCandidates.Add(new(action, weightedScore));

            float CalculateWeight(ResourceType type)
            {
                ResourceManager resMgr = ResourceManager.Instance;
                ResourceStat chosenResource = resMgr.GetResource(type, _actorCtx.Actor);

                float lowerBound = Mathf.Lerp(
                    chosenResource.LowerThreshold,
                    chosenResource.UpperThreshold - 1,
                    chosenResource.Weight * _weight
                );

                float variance = chosenResource.Amount - lowerBound;

                float upperBound = chosenResource.IsUnbounded()
                    ? chosenResource.UpperThreshold
                    : chosenResource.GetMaxValue();

                float ratio = variance / (upperBound - lowerBound);
                float weightedScore = 1 - Math.Clamp(ratio, 0, 1);

                return weightedScore;
            }

            BaseAction CreateAction(ActionType actionType, ActorTag2D chosenActor,
                ResourceType resType, Crime crime)
            {
                return actionType switch
                {
                    ActionType.Theft => new TheftAction(_actorCtx, chosenActor, resType),
                    ActionType.Petition => new PetitionAction(_actorCtx, chosenActor, resType),
                    ActionType.Eat => new EatAction(_actorCtx),
                    ActionType.Socialize => new SocializeAction(_actorCtx),
                    ActionType.Assess => new AssessAction(_actorCtx),
                    ActionType.Interrogate => new InterrogateAction(_actorCtx, chosenActor, crime),
                    ActionType.Pursuit => new PursuitAction(_actorCtx),
                    ActionType.Plant => new PlantAction(_actorCtx),
                    ActionType.Harvest => new HarvestAction(_actorCtx),
                    _ => throw new ArgumentException("Invalid action type"),
                };
            }
        }

        public virtual void Update(double delta) { }

        // ! Remove in production
        public virtual BaseAction EvaluateActionStub(Type actionType, ResourceType resType)
        {
            return null;
        }
    }
}