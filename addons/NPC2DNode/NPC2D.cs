using EmergentEchoes.Entities.Actors.NPCs;
using EmergentEchoes.Utilities.Components;
using EmergentEchoes.Utilities.Internal;
using EmergentEchoes.Utilities.Records;
using EmergentEchoes.Utilities.Traits;
using Godot;
using System;
using System.Collections.Generic;

namespace EmergentEchoes.addons.NPC2DNode
{
    [Tool]
    public partial class NPC2D : NPC
    {
        [Export(PropertyHint.Range, "0,1000000,")]
        public int MoneyValue { get; set; } = 10;
        [Export(PropertyHint.Range, "1,100,")]
        public int FoodValue { get; set; } = 100;
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

        private Strategizer _strategizer;
        private readonly Executor _executor;

        public override void _Ready()
        {
            if (Engine.IsEditorHint()) return;

            base._Ready();

            AddResources();
            AddTraits();
        }

        private void AddTraits()
        {
            List<Trait> traits = new()
            {
                new SurvivalTrait(this, Survival)
            };

            if (Thief > 0)
                traits.Add(new ThiefTrait(this, Thief));

            if (Lawful > 0)
                traits.Add(new LawfulTrait(this, Lawful));

            _strategizer = new(traits);
        }

        private void AddResources()
        {
            Resources = new()
            {
                new ResourceStat(ResourceStat.Stat.Money, Money, true),
                new ResourceStat(ResourceStat.Stat.Food, Food, true),
                new ResourceStat(ResourceStat.Stat.Companionship, Companionship, false),
            };
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