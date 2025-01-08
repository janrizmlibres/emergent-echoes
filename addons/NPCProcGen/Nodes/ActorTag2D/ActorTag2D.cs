using Godot;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NPCProcGen
{
    [Tool]
    public partial class ActorTag2D : Node
    {
        [Export(PropertyHint.Range, "0,1000000,")]
        public int MoneyValue { get; set; } = 10;
        [Export(PropertyHint.Range, "1,100,")]
        public int FoodValue { get; set; } = 100;

        // TODO: Consider converting resources into dictionary for constant access
        public Dictionary<ResourceType, ResourceStat> Resources { get; private set; } = new();

        protected Node2D _parent;

        public override void _Ready()
        {
            if (Engine.IsEditorHint()) return;

            if (_parent == null)
            {
                QueueFree();
                return;
            }

            // TODO: Consider implementing a resource manager
            Resources.Add(ResourceType.Money, new ResourceStat(ResourceType.Money, MoneyValue, 1));
            Resources.Add(ResourceType.Food, new ResourceStat(ResourceType.Food, FoodValue, 1));
        }

        public override void _EnterTree()
        {
            if (Engine.IsEditorHint())
            {
                CheckParent();
            }
        }

        public override string[] _GetConfigurationWarnings()
        {
            List<string> warnings = new();

            if (_parent == null)
            {
                warnings.Add("The ActorTag2D can be used only under a Node2D inheriting parent node.");
            }

            return warnings.ToArray();
        }

        public Vector2 GetParentGlobalPosition()
        {
            return _parent.GlobalPosition;
        }

        protected void CheckParent()
        {
            _parent = GetParent() as Node2D;
            UpdateConfigurationWarnings();

            if (_parent == null)
            {
                SetProcess(false);
                SetPhysicsProcess(false);
            }
            else
            {
                SetProcess(true);
                SetPhysicsProcess(true);
            }
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