using EmergentEchoes.Utilities.Components;
using Godot;
using System;
using System.Collections.Generic;

namespace EmergentEchoes.Entities.Actors
{
    public abstract partial class Actor : CharacterBody2D
    {
        [Export(PropertyHint.Range, "0,1000000,")]
        public int MoneyValue { get; set; } = 10;
        [Export(PropertyHint.Range, "1,100,")]
        public int FoodValue { get; set; } = 100;

        [Export]
        public string ActorName { get; set; }

        // TODO: Add runtime check if actor name is not set

        // TODO: Consider converting resources into dictionary for constant access
        public List<ResourceStat> Resources { get; protected set; } = new();

        protected Dictionary<Actor, float> Relationships { get; set; } = new();

        public bool HasResource(ResourceStat resource)
        {
            ResourceStat foundResource = Resources
                .Find(r => r.Equals(resource))
                ?? throw new Exception("Resource not found.");

            return foundResource.Value > 0;
        }

        public void InitializeRelationships(List<Actor> others)
        {
            foreach (Actor actor in others)
            {
                Relationships.Add(actor, 0);
            }
        }
    }
}