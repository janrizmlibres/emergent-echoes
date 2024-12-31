using EmergentEchoes.Utilities.Internal;
using EmergentEchoes.Utilities.World;
using Godot;
using System;

namespace EmergentEchoes.Utilities.Traits
{
    public abstract class Trait
    {
        protected readonly Sensor _sensor;
        protected readonly Memorizer _memorizer;

        public Trait()
        {
            _sensor = new Sensor();
            _memorizer = new Memorizer();
        }

        public abstract Tuple<string, float> EvaluateAction();
        public abstract bool ShouldActivate(SocialPractice practice);
    }
}