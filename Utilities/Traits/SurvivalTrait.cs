using EmergentEchoes.addons.NPC2DNode;
using EmergentEchoes.Utilities.Actions;
using EmergentEchoes.Utilities.Components.Enums;
using EmergentEchoes.Utilities.Internal;
using Godot;
using System;

namespace EmergentEchoes.Utilities.Traits
{
    public class SurvivalTrait : Trait
    {
        public SurvivalTrait(NPC2D owner, Memorizer memorizer, float weight) : base(owner, memorizer, weight) { }

        public override Tuple<NPCAction, float> EvaluateAction()
        {
            return new(null, 0);
        }

        public override bool ShouldActivate(SocialPractice practice)
        {
            return practice == SocialPractice.Proactive;
        }
    }
}