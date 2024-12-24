using Godot;
using System;
using System.Collections.Generic;

namespace EmergentEchoes
{
	/// <summary>
	/// Represents the state of the game world.
	/// This class is a singleton and provides methods to read and modify the world state.
	/// </summary>
	public sealed partial class WorldState : Node
	{
		/// <summary>
		/// Lazy-loaded instance of the WorldState class.
		/// </summary>
		private static readonly Lazy<WorldState> _lazy = new(() => new WorldState());

		/// <summary>
		/// Gets the singleton instance of the WorldState class.
		/// </summary>
		private static WorldState Instance { get { return _lazy.Value; } }

		/// <summary>
		/// List of NPCs (Non-Player Characters) in the world.
		/// </summary>
		private readonly List<CharacterBody2D> _npcList;

		/// <summary>
		/// Initializes a new instance of the <see cref="WorldState"/> class.
		/// Private constructor to prevent external instantiation.
		/// </summary>
		private WorldState()
		{
			_npcList = new();
		}

		/// <summary>
		/// Interface for reading the world state.
		/// </summary>
		public interface IWorldStateReader
		{
			/// <summary>
			/// Gets the list of NPCs in the world.
			/// </summary>
			/// <returns>A list of <see cref="CharacterBody2D"/> representing the NPCs.</returns>
			List<CharacterBody2D> GetNPCList();
		}

		/// <summary>
		/// Interface for modifying the world state.
		/// </summary>
		public interface IWorldStateWriter
		{
			/// <summary>
			/// Adds an NPC to the world.
			/// </summary>
			/// <param name="npc">The NPC to add.</param>
			void AddNPC(CharacterBody2D npc);

			/// <summary>
			/// Removes an NPC from the world.
			/// </summary>
			/// <param name="npc">The NPC to remove.</param>
			void RemoveNPC(CharacterBody2D npc);
		}

		/// <summary>
		/// Class for reading the world state.
		/// </summary>
		private class WorldStateReader : IWorldStateReader
		{
			/// <summary>
			/// Gets the list of NPCs in the world.
			/// </summary>
			/// <returns>A list of <see cref="CharacterBody2D"/> representing the NPCs.</returns>
			public List<CharacterBody2D> GetNPCList() => Instance._npcList;
		}

		/// <summary>
		/// Class for modifying the world state.
		/// </summary>
		private class WorldStateWriter : IWorldStateWriter
		{
			/// <summary>
			/// Adds an NPC to the world.
			/// </summary>
			/// <param name="npc">The NPC to add.</param>
			public void AddNPC(CharacterBody2D npc) => Instance._npcList.Add(npc);

			/// <summary>
			/// Removes an NPC from the world.
			/// </summary>
			/// <param name="npc">The NPC to remove.</param>
			public void RemoveNPC(CharacterBody2D npc) => Instance._npcList.Remove(npc);
		}

		// TODO: Implement factory pattern for reader and writer

		/// <summary>
		/// Gets a reader for the world state.
		/// </summary>
		/// <param name="caller">The caller requesting the reader. Must be a <see cref="Sensor"/> instance.</param>
		/// <returns>An instance of <see cref="IWorldStateReader"/>.</returns>
		/// <exception cref="UnauthorizedAccessException">Thrown if the caller is not a <see cref="Sensor"/> instance.</exception>
		public static IWorldStateReader GetReader(Sensor caller)
		{
			if (caller == null)
			{
				throw new UnauthorizedAccessException("Only Sensor instances can get a reader.");
			}
			return new WorldStateReader();
		}

		/// <summary>
		/// Gets a writer for the world state.
		/// </summary>
		/// <param name="caller">The caller requesting the writer. Must be a <see cref="World"/> instance.</param>
		/// <returns>An instance of <see cref="IWorldStateWriter"/>.</returns>
		/// <exception cref="UnauthorizedAccessException">Thrown if the caller is not a <see cref="World"/> instance.</exception>
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