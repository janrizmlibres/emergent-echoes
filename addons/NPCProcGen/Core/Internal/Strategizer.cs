using NPCProcGen.Core.Actions;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Traits;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NPCProcGen.Core.Internal
{
    /// <summary>
    /// The Strategizer class is responsible for evaluating and selecting actions based on traits.
    /// </summary>
    public class Strategizer
    {
        private readonly ActorContext _context;

        public Strategizer(ActorContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Evaluates and selects the best action based on the given social practice.
        /// </summary>
        /// <param name="practice">The social practice to evaluate actions for.</param>
        /// <returns>The best action based on the traits, or null if no action is suitable.</returns>
        public BaseAction EvaluateAction(SocialPractice practice)
        {
            NPCAgent2D npcActor = _context.Actor as NPCAgent2D;

            IEnumerable<Tuple<BaseAction, float>> actions = npcActor.Traits
                .Select(trait => trait.EvaluateAction(practice))
                .Where(action => action != null)
                .OrderByDescending(action => action.Item2);

            return actions.Any() ? actions.First().Item1 : null;
        }

        public BaseAction EvaluateActionStub(Type traitType, Type actionType, ResourceType resType)
        {
            NPCAgent2D agent = _context.GetNPCAgent2D();

            foreach (Trait trait in agent.Traits)
            {
                if (trait.GetType() == traitType)
                {
                    return trait.EvaluateActionStub(actionType, resType);
                }
            }
            return null;
        }
    }
}