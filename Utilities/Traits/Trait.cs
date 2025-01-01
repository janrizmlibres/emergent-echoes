using EmergentEchoes.addons.NPC2DNode;
using EmergentEchoes.Utilities.Actions;
using EmergentEchoes.Utilities.Components;
using EmergentEchoes.Utilities.Internal;
using Godot;
using System;

namespace EmergentEchoes.Utilities.Traits
{
    public abstract class Trait
    {
        protected readonly NPC2D _owner;
        protected readonly float _weight;

        protected readonly Sensor _sensor;
        protected readonly Memorizer _memorizer;

        public Trait(NPC2D owner, float weight)
        {
            _owner = owner;
            _weight = weight;

            _sensor = new Sensor();
            _memorizer = new Memorizer();
        }

        public abstract Tuple<NPCAction, float> EvaluateAction();
        public abstract bool ShouldActivate(SocialPractice practice);
    }
}