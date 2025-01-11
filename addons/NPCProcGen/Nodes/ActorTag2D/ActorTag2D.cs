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

        [Export]
        public Marker2D StealMarker
        {
            get => _stealMarker;
            set
            {
                if (value != _stealMarker)
                {
                    _stealMarker = value;
                    UpdateConfigurationWarnings();
                }
            }
        }

        // TODO: Consider converting resources into dictionary for constant access
        public Dictionary<ResourceType, ResourceStat> Resources { get; private set; } = new();

        public Node2D Parent { get; protected set; }

        private Marker2D _stealMarker;

        public override void _Ready()
        {
            if (Engine.IsEditorHint()) return;

            Parent = GetParent() as Node2D;

            if (Parent == null || _stealMarker == null)
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
                Parent = GetParent() as Node2D;
                UpdateConfigurationWarnings();
            }
        }

        public override string[] _GetConfigurationWarnings()
        {
            List<string> warnings = new();

            if (Parent == null)
            {
                warnings.Add("The ActorTag2D can be used only under a Node2D inheriting parent node.");
            }

            if (_stealMarker == null)
            {
                warnings.Add("The ActorTag2D requires a Marker2D node for use in actions such as stealing.");
            }

            return warnings.ToArray();
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