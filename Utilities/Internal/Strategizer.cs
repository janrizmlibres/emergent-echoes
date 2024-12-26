using EmergentEchoes.Utilities.Traits;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EmergentEchoes.Utilities.Internal
{
    public partial class Strategizer : Node
    {
        public static string SelectAction(List<ITrait> traits)
        {
            List<Tuple<float, string>> actions = traits
                .Where(action => action.ShouldActivate())
                .Select(trait => trait.EvaluateAction())
                .Where(action => action.Item1 != 0)
                .OrderByDescending(action => action.Item1)
                .ToList();

            return actions.Any() ? actions.First().Item2 : string.Empty;
        }
    }
}