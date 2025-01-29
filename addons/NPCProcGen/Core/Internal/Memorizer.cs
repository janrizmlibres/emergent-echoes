using Godot;
using System.Collections.Generic;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Events;
using NPCProcGen.Core.Helpers;
using System.Linq;

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

        public List<ActorTag2D> GetPeerActors()
        {
            return _actorData.Keys.ToList();
        }

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

        public void ModifyRelationship(ActorTag2D actor, float amount)
        {
            DebugTool.Assert(_actorData.ContainsKey(actor), $"Actor {actor.Parent.Name} not found in memorizer.");
            _actorData[actor].Relationship += amount;
            GD.Print($"Altered relationship with {actor.Parent.Name} by {amount}. New relationship: {_actorData[actor].Relationship}");
        }

        /// <summary>
        /// Gets the last known location of the specified actor.
        /// </summary>
        /// <param name="actor">The actor to get the location for.</param>
        /// <returns>The last known location of the actor, or null if not found.</returns>
        public Vector2? GetLastActorLocation(ActorTag2D actor)
        {
            return _actorData[actor].LastKnownPosition;
        }

        public float GetActorRelationship(ActorTag2D actor)
        {
            return _actorData[actor].Relationship;
        }

        /// <summary>
        /// Determines if the specified actor is friendly.
        /// </summary>
        /// <param name="actor">The actor to check.</param>
        /// <returns>True if the actor is friendly, otherwise false.</returns>
        public bool IsFriendly(ActorTag2D actor)
        {
            return _actorData[actor].Relationship >= 5;
        }

        /// <summary>
        /// Determines if the specified actor is trusted.
        /// </summary>
        /// <param name="actor">The actor to check.</param>
        /// <returns>True if the actor is trusted, otherwise false.</returns>
        public bool IsTrusted(ActorTag2D actor)
        {
            return _actorData[actor].Relationship >= 15;
        }

        /// <summary>
        /// Determines if the specified actor is close.
        /// </summary>
        /// <param name="actor">The actor to check.</param>
        /// <returns>True if the actor is close, otherwise false.</returns>
        public bool IsClose(ActorTag2D actor)
        {
            return _actorData[actor].Relationship >= 25;
        }
    }
}

// * Hostile: -35 to -26
// * Distrusted: -25 to -16
// * Unfriendly: -15 to -6
// * Neutral: -5 to 4
// * Friendly: 5 to 14
// * Trusted: 15 to 24
// * Close: 25 to 35