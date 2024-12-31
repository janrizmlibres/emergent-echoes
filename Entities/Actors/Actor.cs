using EmergentEchoes.Utilities.World;
using Godot;
using System;
using System.Collections.Generic;

namespace EmergentEchoes.Entities.Actors
{
    public partial class Actor : CharacterBody2D
    {
        // TODO: Consider converting resources into dictionary for constant access
        public List<ResourceStat> Resources { get; protected set; }

        public bool HasResource(ResourceStat resource)
        {
            ResourceStat foundResource = Resources
                .Find(r => r.Equals(resource))
                ?? throw new Exception("Resource not found.");

            return foundResource.Value > 0;
        }
    }
}