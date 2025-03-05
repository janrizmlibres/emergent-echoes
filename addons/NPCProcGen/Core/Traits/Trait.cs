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
    public class ActionParams
    {
        public ActorTag2D TargetActor { get; set; }
        public ActorTag2D Criminal { get; set; }
        public Crime Crime { get; set; }
        public bool CaseClosed { get; set; } = false;
    }

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

            switch (practice)
            {
                case SocialPractice.Proactive:
                    EvaluateProactiveAction();
                    break;
            }

            return _actionCandidates
                .OrderByDescending(action => action.Item2)
                .FirstOrDefault();
        }

        protected abstract void EvaluateProactiveAction();

        protected void EvaluateInteraction(ActionType actionType, ResourceType resType,
            Func<List<ActorTag2D>, ActorTag2D> alternator)
        {
            ActorTag2D chosenActor = ChooseActor();
            if (chosenActor == null) return;

            ActionParams actionParams = new() { TargetActor = chosenActor };
            AddAction(actionType, resType, actionParams);

            ActorTag2D ChooseActor()
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
                        if (actor.Sensor.GetPetitionResourceType() == resType) continue;
                        if (!_actorCtx.Memorizer.IsValidPetitionTarget(actor, resType)) continue;
                    }

                    if (!ResourceManager.Instance.HasResource(resType, actor)) continue;
                    potentialActors.Add(actor);
                }

                return alternator(potentialActors);
            }
        }

        protected void AddAction(ActionType actionType, ResourceType resType,
            ActionParams @params = null)
        {
            float weightedScore = CalculateWeight();
            BaseAction action = CreateAction();
            _actionCandidates.Add(new(action, weightedScore));

            float CalculateWeight()
            {
                ResourceManager resMgr = ResourceManager.Instance;
                ResourceStat chosenResource = resMgr.GetResource(resType, _actorCtx.Actor);

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

            BaseAction CreateAction()
            {
                ActorTag2D targetActor = @params?.TargetActor;
                Crime crime = @params?.Crime;

                return actionType switch
                {
                    ActionType.Theft => new TheftAction(_actorCtx, targetActor, resType),
                    ActionType.Petition => new PetitionAction(_actorCtx, targetActor, resType),
                    ActionType.Eat => new EatAction(_actorCtx),
                    ActionType.Socialize => new SocializeAction(_actorCtx),
                    ActionType.Assess => new AssessAction(
                        _actorCtx,
                        @params.Crime,
                        @params.CaseClosed
                    ),
                    ActionType.Interrogate => new InterrogateAction(
                        _actorCtx,
                        targetActor,
                        crime
                    ),
                    ActionType.Pursuit => new PursuitAction(_actorCtx, @params.Criminal, crime),
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