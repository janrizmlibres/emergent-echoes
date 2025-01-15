using System;
using NPCProcGen.Core.Actions;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.Traits
{
    public class LawfulTrait : Trait
    {
        public LawfulTrait(NPCAgent2D owner, float weight, Sensor sensor, Memorizer memorizer)
            : base(owner, weight, sensor, memorizer) { }

        public override Tuple<NPCAction, float> EvaluateAction(SocialPractice practice)
        {
            return null;
        }
    }
}