using EmergentEchoes.addons.NPC2DNode;
using EmergentEchoes.Utilities.Actions;
using EmergentEchoes.Utilities.Components.Enums;
using EmergentEchoes.Utilities.Internal;
using Godot;
using System;

namespace EmergentEchoes.Utilities.Traits
{
    public class LawfulTrait : Trait
    {
        public LawfulTrait(NPC2D owner, Memorizer memorizer, float weight) : base(owner, memorizer, weight) { }

        public override Tuple<NPCAction, float> EvaluateAction()
        {
            return new(null, 0);
            // return Tuple<NPCAction, float>(new TheftAction(), 0);
        }

        public override bool ShouldActivate(SocialPractice practice)
        {
            return practice == SocialPractice.Proactive;
        }
    }
}