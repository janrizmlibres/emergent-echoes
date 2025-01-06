using System;
using NPCProcGen.Core.Actions;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.Traits
{
    public class LawfulTrait : Trait
    {
        public LawfulTrait(ActorTag2D owner, float weight, Sensor sensor, Memorizer memorizer)
            : base(owner, weight, sensor, memorizer) { }

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