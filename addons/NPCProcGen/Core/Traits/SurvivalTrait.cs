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
        public SurvivalTrait(NPCAgent2D owner, float weight, Sensor sensor, NPCMemorizer memorizer)
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
            ResourceManager resourceMgr = ResourceManager.Instance;
            List<Tuple<BaseAction, float>> actionCandidates = new();

            foreach (ResourceType type in resourceMgr.TangibleTypes)
            {
                if (resourceMgr.IsDeficient(_owner, type))
                {
                    EvaluateInteraction(
                        actionCandidates, type,
                        actor => _memorizer.IsFriendly(actor),
                        (peerActors) => FindActorWithoutBond(peerActors, type),
                        (chosenActor) => () => new PetitionAction(_owner, chosenActor, type)
                    );
                }
            }

            if (resourceMgr.IsDeficient(_owner, ResourceType.Satiation)
                && resourceMgr.HasResource(_owner, ResourceType.Food))
            {
                AddAction(actionCandidates, ResourceType.Satiation, () => new EatAction(_owner));
            }

            if (resourceMgr.IsDeficient(_owner, ResourceType.Companionship))
            {
                AddAction(
                    actionCandidates,
                    ResourceType.Companionship,
                    () => new SocializeAction(_owner)
                );
            }

            return actionCandidates.OrderByDescending(action => action.Item2).FirstOrDefault();
        }

        // ! Remove in production
        public override BaseAction EvaluateActionStub(Type actionType, ResourceType resType)
        {
            List<Tuple<BaseAction, float>> actionCandidates = new();

            if (actionType == typeof(PetitionAction))
            {
                EvaluateInteraction(
                    actionCandidates, resType,
                    actor => _memorizer.IsFriendly(actor),
                    (peerActors) => FindActorWithoutBond(peerActors, resType),
                    (chosenActor) => () => new PetitionAction(_owner, chosenActor, resType)
                );

                return actionCandidates.FirstOrDefault()?.Item1;
            }

            if (actionType == typeof(EatAction) &&
                ResourceManager.Instance.HasResource(_owner, ResourceType.Food))
            {
                AddAction(actionCandidates, ResourceType.Satiation, () => new EatAction(_owner));
                return actionCandidates.FirstOrDefault()?.Item1;
            }

            if (actionType == typeof(SocializeAction))
            {
                AddAction(
                    actionCandidates,
                    ResourceType.Companionship,
                    () => new SocializeAction(_owner)
                );

                return actionCandidates.FirstOrDefault()?.Item1;
            }

            return null;
        }

        private static ActorTag2D FindActorWithoutBond(List<ActorTag2D> peerActors, ResourceType type)
        {
            return FindActorWithoutDeficiency(peerActors, type)
                ?? FindActorWithResource(peerActors, type) ?? null;
        }

        private static ActorTag2D FindActorWithoutDeficiency(List<ActorTag2D> peerActors, ResourceType type)
        {
            foreach (ActorTag2D actor in peerActors)
            {
                if (!ResourceManager.Instance.IsDeficient(actor, type))
                {
                    return actor;
                }
            }

            return null;
        }

        private static ActorTag2D FindActorWithResource(List<ActorTag2D> peerActors, ResourceType type)
        {
            foreach (ActorTag2D actor in peerActors)
            {
                if (ResourceManager.Instance.HasResource(actor, type))
                {
                    return actor;
                }
            }

            return null;
        }
    }
}