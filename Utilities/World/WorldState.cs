using EmergentEchoes.Utilities.Internal;
using Godot;
using System;
using System.Collections.Generic;

namespace EmergentEchoes
{
	public sealed partial class WorldState : Node
	{
		private static readonly Lazy<WorldState> _lazy = new(() => new WorldState());

		private static WorldState Instance { get { return _lazy.Value; } }

		private readonly List<CharacterBody2D> _npcList;

		private WorldState()
		{
			_npcList = new();
		}

		public interface IWorldStateReader
		{
			List<CharacterBody2D> GetNPCList();
		}

		public interface IWorldStateWriter
		{
			void AddNPC(CharacterBody2D npc);

			void RemoveNPC(CharacterBody2D npc);
		}

		private class WorldStateReader : IWorldStateReader
		{
			public List<CharacterBody2D> GetNPCList() => Instance._npcList;
		}

		private class WorldStateWriter : IWorldStateWriter
		{
			public void AddNPC(CharacterBody2D npc) => Instance._npcList.Add(npc);

			public void RemoveNPC(CharacterBody2D npc) => Instance._npcList.Remove(npc);
		}

		// TODO: Implement factory pattern for reader and writer

		public static IWorldStateReader GetReader(Sensor caller)
		{
			if (caller == null)
			{
				throw new UnauthorizedAccessException("Only Sensor instances can get a reader.");
			}
			return new WorldStateReader();
		}

		public static IWorldStateWriter GetWriter(World caller)
		{
			if (caller == null)
			{
				throw new UnauthorizedAccessException("Only World instances can get a writer.");
			}
			return new WorldStateWriter();
		}
	}
}