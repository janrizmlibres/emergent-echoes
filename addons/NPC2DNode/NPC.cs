using EmergentEchoes.Entities.Actors;
using EmergentEchoes.Utilities.Actions;
using EmergentEchoes.Utilities.Internal;
using EmergentEchoes.Utilities.Traits;
using EmergentEchoes.Utilities.World;
using Godot;
using System;
using System.Collections.Generic;

namespace EmergentEchoes.addons.NPC2DNode
{
    public partial class NPC : Actor
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

        public List<Trait> Traits { get; private set; }

        private Strategizer _strategizer;
        private Executor _executor;

        private readonly NPCAction _currentAction;

        public override void _Ready()
        {
            AddResources();
            AddTraits();
        }

        private void AddTraits()
        {
            Traits = new()
            {
                new SurvivalTrait(Survival)
            };

            if (Thief > 0)
            {
                Traits.Add(new ThiefTrait(this, Resources, Thief));
            }

            if (Lawful > 0)
                Traits.Add(new LawfulTrait(Lawful));

            _strategizer = new(Traits);
        }

        private void AddResources()
        {
            Resources = new()
            {
                new ResourceStat(ResourceStat.Type.Money, Money, true),
                new ResourceStat(ResourceStat.Type.Food, Food, true),
                new ResourceStat(ResourceStat.Type.Companionship, Companionship, false),
                new ResourceStat(ResourceStat.Type.Social, Social, false),
            };
        }
    }
}