using EmergentEchoes.addons.NPC2DNode;
using EmergentEchoes.Utilities.Actions;
using EmergentEchoes.Utilities.Components;
using Godot;
using System;

namespace EmergentEchoes.Utilities.Traits
{
    public class LawfulTrait : Trait
    {
        public LawfulTrait(NPC2D owner, float weight) : base(owner, weight) { }

        public override Tuple<NPCAction, float> EvaluateAction()
        {
            return new(null, 0);
        }

        public override bool ShouldActivate(SocialPractice practice)
        {
            return practice.Type == SocialPractice.Practice.Proactive;
        }
    }
}