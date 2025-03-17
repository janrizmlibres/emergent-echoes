using Godot;
using NPCProcGen.Core.Actions;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Traits;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NPCProcGen.Core.Internal
{
    public class Strategizer
    {
        private readonly ActorContext _context;

        public Strategizer(ActorContext context)
        {
            _context = context;
        }

        public BaseAction EvaluateAction(SocialPractice practice)
        {
            NPCAgent2D npcActor = _context.Actor as NPCAgent2D;

            List<(BaseAction, float)> evaluations = npcActor.Traits
                .Select(trait => trait.EvaluateAction(practice))
                .Where(eval => eval != null)
                .Where(eval => GD.Randf() <= eval.Item2)
                .Select(eval => (eval.Item1, (float)Math.Round(eval.Item2, 2)))
                .ToList();

            if (!evaluations.Any())
                return null;

            var maxWeight = evaluations.Max(eval => eval.Item2);
            var maxWeightedItems = evaluations
                .Where(eval => eval.Item2 == maxWeight)
                .ToList();

            return maxWeightedItems.Any()
                ? maxWeightedItems[(int)(GD.Randi() % maxWeightedItems.Count)].Item1
                : null;
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