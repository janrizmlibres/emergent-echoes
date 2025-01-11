using System;
using NPCProcGen.Core.Actions;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.Traits
{
    public abstract class Trait
    {
        protected readonly NPCAgent2D _owner;
        protected readonly float _weight;

        protected readonly Sensor _sensor;
        protected readonly Memorizer _memorizer;

        public Trait(NPCAgent2D owner, float weight, Sensor sensor, Memorizer memorizer)
        {
            _owner = owner;
            _weight = weight;

            _sensor = sensor;
            _memorizer = memorizer;
        }

        // ! Remove this debug function in production
        public string GetOwnerName()
        {
            return _owner.Parent.Name;
        }

        public abstract Tuple<NPCAction, float> EvaluateAction();
        public abstract bool ShouldActivate(SocialPractice practice);
    }
}