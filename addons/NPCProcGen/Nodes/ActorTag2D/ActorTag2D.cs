using Godot;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using System.Collections.Generic;

namespace NPCProcGen
{
    [Tool]
    public partial class ActorTag2D : Node
    {
        [Export(PropertyHint.Range, "0,1000000,")]
        public int MoneyValue { get; set; } = 10;
        [Export(PropertyHint.Range, "1,100,")]
        public int FoodValue { get; set; } = 100;

        protected Node2D _parent;

        // TODO: Consider converting resources into dictionary for constant access
        public Dictionary<ResourceType, ResourceStat> Resources { get; private set; } = new();

        public override void _Ready()
        {
            if (Engine.IsEditorHint()) return;

            _parent = GetParent() as Node2D;

            // TODO: Consider implementing a resource manager

            Resources.Add(ResourceType.Money, new ResourceStat(ResourceType.Money, MoneyValue, 1));
            Resources.Add(ResourceType.Food, new ResourceStat(ResourceType.Food, FoodValue, 1));
        }

        public bool HasResource(ResourceType type)
        {
            if (Resources.TryGetValue(type, out ResourceStat resource))
            {
                return resource.Value > 0;
            }

            return false;
        }
    }
}