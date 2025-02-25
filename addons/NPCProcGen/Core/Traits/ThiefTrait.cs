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
        public ThiefTrait(ActorContext context, float weight) : base(context, weight) { }

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
            ResourceManager resourceMgr = ResourceManager.Instance;
            List<Tuple<BaseAction, float>> actionCandidates = new();

            foreach (ResourceType type in resourceMgr.TangibleTypes)
            {
                if (resourceMgr.IsDeficient(_actorCtx.Actor, type))
                {
                    EvaluateInteraction(
                        actionCandidates, type,
                        peerActors => PickActor(peerActors, type),
                        ActionType.Theft
                    );
                }
            }

            return actionCandidates.OrderByDescending(tuple => tuple.Item2).FirstOrDefault();
        }

        // ! Remove in production
        public override BaseAction EvaluateActionStub(Type actionType, ResourceType resType)
        {
            List<Tuple<BaseAction, float>> actionCandidates = new();

            EvaluateInteraction(
                actionCandidates, resType,
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
                    || !ResourceManager.Instance.IsDeficient(actor, type))
                {
                    return actor;
                }
            }

            return peerActors.FirstOrDefault();
        }
    }
}