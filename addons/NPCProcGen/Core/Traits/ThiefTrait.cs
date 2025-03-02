using System;
using System.Collections.Generic;
using System.Linq;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Actions;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.Traits
{
    public class ThiefTrait : Trait
    {
        public ThiefTrait(ActorContext context, float weight) : base(context, weight) { }

        protected override void EvaluateProactiveAction()
        {
            ResourceManager resourceMgr = ResourceManager.Instance;

            foreach (ResourceType type in resourceMgr.TangibleTypes)
            {
                EvaluateInteraction(
                    type,
                    peerActors => PickActor(peerActors, type),
                    ActionType.Theft
                );
            }
        }

        // ! Remove in production
        public override BaseAction EvaluateActionStub(Type actionType, ResourceType resType)
        {
            List<Tuple<BaseAction, float>> actionCandidates = new();

            EvaluateInteraction(
                resType,
                peerActors => PickActor(peerActors, resType),
                ActionType.Theft
            );

            return actionCandidates.FirstOrDefault()?.Item1;
        }

        private ActorTag2D PickActor(List<ActorTag2D> peerActors, ResourceType type)
        {
            foreach (ActorTag2D actor in peerActors)
            {
                if (!_actorCtx.Memorizer.IsTrusted(actor)
                    || !ResourceManager.Instance.IsDeficient(type, actor))
                {
                    return actor;
                }
            }

            return peerActors.FirstOrDefault();
        }
    }
}