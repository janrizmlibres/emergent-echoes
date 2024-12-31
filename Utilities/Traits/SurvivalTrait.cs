using EmergentEchoes.Utilities.World;
using Godot;
using System;

namespace EmergentEchoes.Utilities.Traits
{
    public class SurvivalTrait : Trait
    {
        private readonly float _weight;

        public SurvivalTrait(float weight)
        {
            _weight = weight;
        }

        public override Tuple<string, float> EvaluateAction()
        {
            // Calculate score based on:
            // - Current needs/resources
            // - Memory of past attempts
            // - Probability of success
            // Return weighted score
            // return score * _weight;
            return new("Survival", 0);
        }

        public override bool ShouldActivate(SocialPractice practice)
        {
            return practice.PracticeType == SocialPractice.Practice.Proactive;
        }
    }
}