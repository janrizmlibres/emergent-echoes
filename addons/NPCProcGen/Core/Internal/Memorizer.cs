using System.Collections.Generic;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Events;

namespace NPCProcGen.Core.Internal
{
    public class Memorizer
    {
        public List<Event> LongTermMemory { get; private set; } = new();

        public LinkedList<Event> ShortTermMemory { get; private set; } = new();

        private readonly Dictionary<ActorTag2D, ActorData> _actorData = new();

        // * Hostile: -15 to -11
        // * Distrusted: -10 to -6
        // * Unfriendly: -5 to -1
        // * Neutral: 0 to 5
        // * Friendly: 6 to 10
        // * Trusted: 11 to 15
        // * Close: 16 to 20

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

        public void AddActorData(ActorTag2D actor)
        {
            _actorData.Add(actor, new ActorData(actor));
        }

        public void UpdateActorData(double delta)
        {
            foreach (ActorData actorData in _actorData.Values)
            {
                actorData.Update(delta);
            }
        }
    }
}