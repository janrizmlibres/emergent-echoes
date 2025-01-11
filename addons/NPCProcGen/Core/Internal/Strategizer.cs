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
        private readonly List<Trait> _traits = new();

        public void AddTrait(Trait trait)
        {
            _traits.Add(trait);
        }

        public NPCAction EvaluateAction(SocialPractice practice)
        {
            List<Tuple<NPCAction, float>> actions = _traits
                .Where(action => action.ShouldActivate(practice))
                .Select(trait => trait.EvaluateAction())
                .Where(action => action.Item2 != 0)
                .OrderByDescending(action => action.Item2)
                .ToList();

            return actions.Any() ? actions.First().Item1 : null;
        }
    }
}