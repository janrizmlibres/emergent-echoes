using Godot;
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
		public int MoneyValue { get; set; } = 50;

		/// <summary>
		/// Gets or sets the food value associated with this actor.
		/// </summary>
		/// <value>The food value as an integer.</value>
		[Export(PropertyHint.Range, "0")]
		public int FoodValue { get; set; } = 20;

		/// <summary>
		/// Gets or sets the StealMarker, which is a Marker2D instance.
		/// When the StealMarker is set to a new value, it updates the configuration warnings.
		/// </summary>
		/// <value>The Marker2D instance representing the StealMarker.</value>
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

		/// <summary>
		/// Gets the parent node as a Node2D.
		/// </summary>
		public Node2D Parent { get; protected set; }

		private Marker2D _stealMarker;

		/// <summary>
		/// Called when the node is added to the scene.
		/// Initializes the parent node and checks for required nodes.
		/// </summary>
		public override void _Ready()
		{
			if (Engine.IsEditorHint()) return;

			Parent = GetParent() as Node2D;

			if (Parent == null || _stealMarker == null)
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

			if (_stealMarker == null)
			{
				warnings.Add("The ActorTag2D requires a Marker2D node for use in actions such as stealing.");
			}

			return warnings.ToArray();
		}
	}
}
