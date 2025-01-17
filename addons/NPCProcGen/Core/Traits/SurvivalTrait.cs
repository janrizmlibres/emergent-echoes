using System;
using NPCProcGen.Core.Actions;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.Traits
{
    public class SurvivalTrait : Trait
    {
        public SurvivalTrait(NPCAgent2D owner, float weight, Sensor sensor, Memorizer memorizer)
            : base(owner, weight, sensor, memorizer) { }

        public override Tuple<BaseAction, float> EvaluateAction(SocialPractice practice)
        {
            return null;
        }
    }
}