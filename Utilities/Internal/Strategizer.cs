using EmergentEchoes.Utilities.Actions;
using EmergentEchoes.Utilities.Components;
using EmergentEchoes.Utilities.Components.Enums;
using EmergentEchoes.Utilities.Traits;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EmergentEchoes.Utilities.Internal
{
    public class Strategizer
    {
        private readonly List<Trait> _traits;

        public Strategizer(List<Trait> traits)
        {
            _traits = traits;
        }

        public NPCAction SelectAction(SocialPractice practice)
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