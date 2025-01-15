using Godot;
using System.Collections.Generic;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Events;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.Internal
{
    public class Memorizer
    {
        public List<Event> LongTermMemory { get; private set; } = new();

        public LinkedList<Event> ShortTermMemory { get; private set; } = new();

        private readonly Dictionary<ActorTag2D, ActorData> _actorData = new();

        public void Initialize(List<ActorTag2D> actors)
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

        public void UpdateActorLocation(ActorTag2D actor, Vector2 location)
        {
            DebugTool.Assert(_actorData.ContainsKey(actor), $"Actor {actor.Parent.Name} not found in memorizer.");
            _actorData[actor].LastKnownPosition = location;
        }

        public Vector2? GetActorLocation(ActorTag2D actor)
        {
            return _actorData[actor].LastKnownPosition;
        }

        public bool IsFriendly(ActorTag2D actor)
        {
            return _actorData[actor].Relationship > 5;
        }

        public bool IsTrusted(ActorTag2D actor)
        {
            return _actorData[actor].Relationship > 10;
        }

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