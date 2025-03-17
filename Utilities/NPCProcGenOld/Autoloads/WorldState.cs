using Godot;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Helpers;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace NPCProcGen.Autoloads
{
	public partial class WorldState : Node
	{
		public WorldState()
		{
			DebugTool.Assert(!_isInstantiated, "WorldState has already been instantiated!");
			_isInstantiated = true;
		}

		~WorldState()
		{
			_isInstantiated = false;
		}

		private bool _isInstantiated = false;

		public Dictionary<ActorTag2D, ActorState> ActorState { get; private set; } = new();

		public List<string> GlobalEvents { get; private set; } = [];

		public List<Crime> Crimes { get; private set; } = [];

		public List<Node2D> Structures { get; private set; } = new();
		public List<PrisonMarker2D> Prisons { get; private set; } = [];
		public List<CropMarker2D> CropTiles { get; private set; } = [];

		// ? Workplaces and who they belong to (Possibly _structures)
		// ? Current date (?)
	}
}