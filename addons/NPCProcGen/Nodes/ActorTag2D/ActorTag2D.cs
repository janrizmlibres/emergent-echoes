using Godot;
using Godot.Collections;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Internal;
using System;
using System.Collections.Generic;

namespace NPCProcGen
{
    /// <summary>
    /// Represents a 2D tag for an actor, which includes monetary and food values, and a steal marker.
    /// </summary>
    [Tool]
    public partial class ActorTag2D : Node
    {
        /// <summary>
        /// Gets or sets the monetary value associated with this actor.
        /// </summary>
        /// <value>The monetary value as an integer.</value>
        [Export(PropertyHint.Range, "0,1000000,")]
        public int MoneyAmount { get; set; } = 50;

        /// <summary>
        /// Gets or sets the food value associated with this actor.
        /// </summary>
        /// <value>The food value as an integer.</value>
        [Export(PropertyHint.Range, "0,1000,")]
        public int FoodAmount { get; set; } = 2;

        /// <summary>
        /// Gets or sets the RearMarker, which is a Marker2D instance.
        /// When the RearMarker is set to a new value, it updates the configuration warnings.
        /// </summary>
        /// <value>The Marker2D instance representing the RearMarker.</value>
        [Export]
        public Marker2D RearMarker
        {
            get => _rearMarker;
            set
            {
                if (value != _rearMarker)
                {
                    _rearMarker = value;
                    UpdateConfigurationWarnings();
                }
            }
        }

        // TODO: Add exported property for character dimensions

        [Signal]
        public delegate void InteractionStartedEventHandler(Variant state, Array<Variant> data);
        [Signal]
        public delegate void InteractionEndedEventHandler();

        /// <summary>
        /// Gets the parent node as a Node2D.
        /// </summary>
        public Node2D Parent { get; protected set; }

        /// <summary>
        /// Gets the notification manager of the Actor.
        /// </summary>
        public NotifManager NotifManager { get; private set; } = new();

        /// <summary>
        /// Gets the memorizer component of the Actor.
        /// </summary>
        public Memorizer Memorizer { get; protected set; }

        private Marker2D _rearMarker;

        /// <summary>
        /// Called when the node is added to the scene.
        /// Initializes the parent node and checks for required nodes.
        /// </summary>
        public override void _Ready()
        {
            if (Engine.IsEditorHint()) return;

            Parent = GetParent() as Node2D;
            Memorizer = new Memorizer();

            if (Parent == null || _rearMarker == null)
            {
                QueueFree();
                return;
            }
        }

        /// <summary>
        /// Called when the node enters the scene tree.
        /// Updates the parent node and configuration warnings if in the editor.
        /// </summary>
        public override void _EnterTree()
        {
            if (Engine.IsEditorHint())
            {
                Parent = GetParent() as Node2D;
                UpdateConfigurationWarnings();
            }
        }

        /// <summary>
        /// Provides configuration warnings for the node.
        /// </summary>
        /// <returns>An array of warning messages.</returns>
        public override string[] _GetConfigurationWarnings()
        {
            List<string> warnings = new();

            if (Parent == null)
            {
                warnings.Add("The ActorTag2D can be used only under a Node2D inheriting parent node.");
            }

            if (_rearMarker == null)
            {
                warnings.Add("The ActorTag2D requires a Marker2D node for use in actions such as stealing.");
            }

            return warnings.ToArray();
        }

        public void AnswerPetition(bool isAccepted)
        {
            NotifManager.NotifyPetitionAnswered(isAccepted);
        }

        public int GetFoodAmount()
        {
            return (int)ResourceManager.Instance.GetResource(this, ResourceType.Food).Amount;
        }

        public void AddFood(int amount)
        {
            ResourceManager.Instance.ModifyResource(this, ResourceType.Food, amount);
        }

        public void DeductFood(int amount)
        {
            ResourceManager.Instance.ModifyResource(this, ResourceType.Food, -amount);
        }
    }
}