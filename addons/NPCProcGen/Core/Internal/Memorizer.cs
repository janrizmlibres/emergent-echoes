using Godot;
using System.Collections.Generic;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Events;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.Internal
{
    /// <summary>
    /// The Memorizer class is responsible for managing long-term and short-term memories of actors.
    /// </summary>
    public class Memorizer
    {
        /// <summary>
        /// Gets the list of long-term memories.
        /// </summary>
        public List<Event> LongTermMemory { get; private set; } = new();

        /// <summary>
        /// Gets the linked list of short-term memories.
        /// </summary>
        public LinkedList<Event> ShortTermMemory { get; private set; } = new();

        /// <summary>
        /// Dictionary to store actor data.
        /// </summary>
        private readonly Dictionary<ActorTag2D, ActorData> _actorData = new();

        /// <summary>
        /// Initializes the memorizer with a list of actors.
        /// </summary>
        /// <param name="actors">The list of actors to initialize.</param>
        public void Initialize(List<ActorTag2D> actors)
        {
            foreach (ActorTag2D actor in actors)
            {
                _actorData.Add(actor, new ActorData(actor));
            }
        }

        /// <summary>
        /// Updates the memorizer with the given delta time.
        /// </summary>
        /// <param name="delta">The delta time to update with.</param>
        public void Update(double delta)
        {
            foreach (ActorData actorData in _actorData.Values)
            {
                actorData.Update(delta);
            }
        }

        // TODO: Fix existing bug
        /// <summary>
        /// Updates the location of the specified actor.
        /// </summary>
        /// <param name="actor">The actor to update the location for.</param>
        /// <param name="location">The new location of the actor.</param>
        public void UpdateActorLocation(ActorTag2D actor, Vector2 location)
        {
            DebugTool.Assert(_actorData.ContainsKey(actor), $"Actor {actor.Parent.Name} not found in memorizer.");
            _actorData[actor].LastKnownPosition = location;
        }

        /// <summary>
        /// Gets the last known location of the specified actor.
        /// </summary>
        /// <param name="actor">The actor to get the location for.</param>
        /// <returns>The last known location of the actor, or null if not found.</returns>
        public Vector2? GetActorLocation(ActorTag2D actor)
        {
            return _actorData[actor].LastKnownPosition;
        }

        /// <summary>
        /// Determines if the specified actor is friendly.
        /// </summary>
        /// <param name="actor">The actor to check.</param>
        /// <returns>True if the actor is friendly, otherwise false.</returns>
        public bool IsFriendly(ActorTag2D actor)
        {
            return _actorData[actor].Relationship > 5;
        }

        /// <summary>
        /// Determines if the specified actor is trusted.
        /// </summary>
        /// <param name="actor">The actor to check.</param>
        /// <returns>True if the actor is trusted, otherwise false.</returns>
        public bool IsTrusted(ActorTag2D actor)
        {
            return _actorData[actor].Relationship > 10;
        }

        /// <summary>
        /// Determines if the specified actor is close.
        /// </summary>
        /// <param name="actor">The actor to check.</param>
        /// <returns>True if the actor is close, otherwise false.</returns>
        public bool IsClose(ActorTag2D actor)
        {
            return _actorData[actor].Relationship > 15;
        }
    }
}

// * Hostile: -15 to -11
// * Distrusted: -10 to -6
// * Unfriendly: -5 to -1
// * Neutral: 0 to 5
// * Friendly: 6 to 10
// * Trusted: 11 to 15
// * Close: 16 to 20