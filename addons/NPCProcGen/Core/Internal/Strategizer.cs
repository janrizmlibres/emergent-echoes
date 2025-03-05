using Godot;
using NPCProcGen.Core.Actions;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Traits;
using System;
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

            return npcActor.Traits
                .Select(trait => trait.EvaluateAction(practice))
                .Where(eval => eval != null)
                .Where(eval => GD.Randf() <= eval.Item2)
                .MaxBy(eval => eval.Item2)?.Item1;
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