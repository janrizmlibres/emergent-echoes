using EmergentEchoes.Utilities.Traits;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EmergentEchoes.Utilities.Internal
{
    public class SocialPractice
    {
        public enum Practice
        {
            Proactive,
            RefusedPetition,
            FailedPetition,
        }
    }

    public partial class Strategizer : Node
    {
        private readonly List<ITrait> _traits = new();

        public Strategizer(List<ITrait> traits)
        {
            _traits = traits;
        }

        public string SelectAction()
        {
            List<Tuple<string, float>> actions = _traits
                .Where(action => action.ShouldActivate())
                .Select(trait => trait.EvaluateAction())
                .Where(action => action.Item2 != 0)
                .OrderByDescending(action => action.Item2)
                .ToList();

            return actions.Any() ? actions.First().Item1 : string.Empty;
        }
    }
}