using EmergentEchoes.Utilities.Traits;
using EmergentEchoes.Utilities.World;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EmergentEchoes.Utilities.Internal
{
    public partial class Strategizer : Node
    {
        private readonly List<Trait> _traits;

        public Strategizer(List<Trait> traits)
        {
            _traits = traits;
        }

        public string SelectAction(SocialPractice practice)
        {
            List<Tuple<string, float>> actions = _traits
                .Where(action => action.ShouldActivate(practice))
                .Select(trait => trait.EvaluateAction())
                .Where(action => action.Item2 != 0)
                .OrderByDescending(action => action.Item2)
                .ToList();

            return actions.Any() ? actions.First().Item1 : string.Empty;
        }
    }
}