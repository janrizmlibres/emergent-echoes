using Godot;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Helpers;
using System.Collections.Generic;

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

		public List<ActorTag2D> Actors { get; private set; } = new();
		public List<PrisonArea2D> Prisons { get; private set; } = new();
		public Dictionary<ActorTag2D, ActorState> ActorState { get; private set; } = new();

		public List<string> GlobalEvents { get; private set; } = new();

		public Dictionary<NPCAgent2D, Crime> Investigations { get; private set; } = new();
		public Queue<Crime> RecordedCrimes { get; private set; } = new();
		public List<Crime> UnsolvedCrimes { get; private set; } = new();
		public List<Crime> SolvedCrimes { get; private set; } = new();

		public List<Node2D> Structures { get; private set; } = new();

		// ? Workplaces and who they belong to (Possibly _structures)
		// ? Current date (?)
	}
}