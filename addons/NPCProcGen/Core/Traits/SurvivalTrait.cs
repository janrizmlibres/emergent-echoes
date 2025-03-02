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
        public SurvivalTrait(ActorContext context, float weight) : base(context, weight) { }

        protected override void EvaluateProactiveAction()
        {
            ResourceManager resMgr = ResourceManager.Instance;

            foreach (ResourceType type in resMgr.TangibleTypes)
            {
                EvaluateInteraction(
                    type,
                    (peerActors) => PickActor(peerActors, type),
                    ActionType.Petition
                );
            }

            if (resMgr.HasResource(ResourceType.Food, _actorCtx.Actor))
            {
                AddAction(ActionType.Eat, ResourceType.Satiation);
            }

            AddAction(
                ActionType.Socialize,
                ResourceType.Companionship
            );
        }

        // ! Remove in production
        public override BaseAction EvaluateActionStub(Type actionType, ResourceType resType)
        {
            List<Tuple<BaseAction, float>> actionCandidates = new();

            if (actionType == typeof(PetitionAction))
            {
                EvaluateInteraction(
                    resType,
                    peerActors => PickActor(peerActors, resType),
                    ActionType.Petition
                );

                return actionCandidates.FirstOrDefault()?.Item1;
            }

            if (actionType == typeof(EatAction) &&
                ResourceManager.Instance.HasResource(ResourceType.Food, _actorCtx.Actor))
            {
                AddAction(ActionType.Eat, ResourceType.Satiation);
                return actionCandidates.FirstOrDefault()?.Item1;
            }

            if (actionType == typeof(SocializeAction))
            {
                AddAction(
                    ActionType.Socialize,
                    ResourceType.Companionship
                );

                return actionCandidates.FirstOrDefault()?.Item1;
            }

            return null;
        }

        private ActorTag2D PickActor(List<ActorTag2D> peerActors, ResourceType type)
        {
            foreach (ActorTag2D actor in peerActors)
            {
                if (_actorCtx.Memorizer.IsFriendly(actor)
                    || !ResourceManager.Instance.IsDeficient(type, actor))
                {
                    return actor;
                }
            }

            return peerActors.FirstOrDefault();
        }
    }
}