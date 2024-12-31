using Godot;
using System;

namespace EmergentEchoes.Utilities.Traits
{
    public class ThiefTrait : ITrait
    {
        private readonly float _weight;

        public ThiefTrait(float weight)
        {
            _weight = weight;
        }

        public Tuple<string, float> EvaluateAction()
        {
            // Calculate score based on:
            // - Current needs/resources
            // - Memory of past attempts
            // - Probability of success
            // Return weighted score
            // return score * _weight;
            return new("Thief", 0);
        }

        public bool ShouldActivate()
        {
            // Check if conditions are right to activate this trait
            // E.g. are we desperate for resources?
            // return conditions;
            return false;
        }
    }
}