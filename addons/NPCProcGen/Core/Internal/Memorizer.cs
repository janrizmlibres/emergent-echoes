using Godot;
using System.Collections.Generic;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Helpers;
using System.Linq;

namespace NPCProcGen.Core.Internal
{
    public class Memorizer
    {
        /// <summary>
        /// Dictionary to store actor data.
        /// </summary>
        protected readonly Dictionary<ActorTag2D, ActorData> _actorData = new();

        public virtual void Initialize(List<ActorTag2D> actors)
        {
            foreach (ActorTag2D actor in actors)
            {
                _actorData.Add(actor, new ActorData(actor));
            }
        }

        public void Update(double delta)
        {
            foreach (ActorData actorData in _actorData.Values)
            {
                actorData.Update(delta);
            }
        }

        public float GetActorRelationship(ActorTag2D actor) => _actorData[actor].Relationship;

        public void UpdateRelationship(ActorTag2D actor, float amount)
        {
            DebugTool.Assert(_actorData.ContainsKey(actor), $"Actor {actor.Parent.Name} not found in memorizer.");
            _actorData[actor].Relationship += amount;
            GD.Print($"Updated relationship with {actor.Parent.Name} by {amount}. New relationship: {_actorData[actor].Relationship}");
        }

        public List<ActorTag2D> GetPeerActors() => _actorData.Keys.ToList();
        public bool IsFriendly(ActorTag2D actor) => _actorData[actor].IsFriendly();
        public bool IsTrusted(ActorTag2D actor) => _actorData[actor].IsTrusted();
        public bool IsClose(ActorTag2D actor) => _actorData[actor].IsClose();

        public virtual Vector2? GetLastKnownPosition(ActorTag2D actor) => null;
        public virtual void UpdateLastKnownPosition(ActorTag2D actor, Vector2 location) { }
    }

    /// <summary>
    /// The Memorizer class is responsible for managing long-term and short-term memories of actors.
    /// </summary>
    public class NPCMemorizer : Memorizer
    {
        /// <summary>
        /// Initializes the memorizer with a list of actors.
        /// </summary>
        /// <param name="actors">The list of actors to initialize.</param>
        public override void Initialize(List<ActorTag2D> actors)
        {
            foreach (ActorTag2D actor in actors)
            {
                _actorData.Add(actor, new NPCActorData(actor));
            }
        }

        /// <summary>
        /// Updates the location of the specified actor.
        /// </summary>
        /// <param name="actor">The actor to update the location for.</param>
        /// <param name="location">The new location of the actor.</param>
        public override void UpdateLastKnownPosition(ActorTag2D actor, Vector2 location)
        {
            NPCActorData npcActorData = _actorData[actor] as NPCActorData;
            npcActorData.LastKnownPosition = location;
        }

        /// <summary>
        /// Gets the last known location of the specified actor.
        /// </summary>
        /// <param name="actor">The actor to get the location for.</param>
        /// <returns>The last known location of the actor, or null if not found.</returns>
        public override Vector2? GetLastKnownPosition(ActorTag2D actor)
        {
            DebugTool.Assert(_actorData[actor] is NPCActorData, "Actor data is not of type NPCActorData.");
            return (_actorData[actor] as NPCActorData)?.LastKnownPosition;
        }
    }
}