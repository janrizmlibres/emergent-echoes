using EmergentEchoes.Entities.Actors;
using EmergentEchoes.Entities.Actors.NPCs;
using EmergentEchoes.Utilities.Components;
using EmergentEchoes.Utilities.Components.Enums;
using EmergentEchoes.Utilities.Internal;
using EmergentEchoes.Utilities.Traits;
using Godot;
using System;
using System.Collections.Generic;

namespace EmergentEchoes.addons.NPC2DNode
{
    [Tool]
    public partial class NPC2D : NPC
    {
        [Export(PropertyHint.Range, "1,100,")]
        public int CompanionshipValue { get; set; } = 100;

        [ExportGroup("Traits")]

        [Export(PropertyHint.Range, "0.01,1,0.01")]
        public float Survival { get; set; } = 0.1f;
        [Export(PropertyHint.Range, "0,1,0.01")]
        public float Thief { get; set; } = 0;
        [Export(PropertyHint.Range, "0,1,0.01")]
        public float Lawful { get; set; } = 0;
        [Export(PropertyHint.Range, "0,1,0.01")]
        public float Generous { get; set; } = 0;
        [Export(PropertyHint.Range, "0,1,0.01")]
        public float Violent { get; set; } = 0;
        [Export(PropertyHint.Range, "0,1,0.01")]
        public float Social { get; set; } = 0;

        [ExportGroup("Resource Weights")]

        [Export(PropertyHint.Range, "0,1,0.01")]
        public float Money { get; set; } = 0.5f;
        [Export(PropertyHint.Range, "0,1,0.01")]
        public float Food { get; set; } = 0.5f;
        [Export(PropertyHint.Range, "0,1,0.01")]
        public float Companionship { get; set; } = 0.5f;

        private readonly Memorizer _memorizer = new();
        private Strategizer _strategizer;
        private readonly Executor _executor;

        public override void _Ready()
        {
            if (Engine.IsEditorHint()) return;

            base._Ready();
            _memorizer.Relationships = Relationships;

            AddResources();
            AddTraits();
        }

        private void AddTraits()
        {
            List<Trait> traits = new()
            {
                new SurvivalTrait(this, _memorizer, Survival)
            };

            if (Thief > 0)
                traits.Add(new ThiefTrait(this, _memorizer, Thief));

            if (Lawful > 0)
                traits.Add(new LawfulTrait(this, _memorizer, Lawful));

            _strategizer = new(traits);
        }

        private void AddResources()
        {
            Resources.Add(new ResourceStat(StatType.Money, Money, true));
            Resources.Add(new ResourceStat(StatType.Food, Food, true));
            Resources.Add(new ResourceStat(StatType.Companionship, Companionship, false));
        }

        public override void _PhysicsProcess(double delta)
        {
            if (Engine.IsEditorHint()) return;

            if (_executor == null)
                base._PhysicsProcess(delta);
            else
                _executor.Update();
        }
    }
}