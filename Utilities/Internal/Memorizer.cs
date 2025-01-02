using EmergentEchoes.addons.NPC2DNode;
using EmergentEchoes.Entities.Actors;
using EmergentEchoes.Utilities.Events;
using Godot;
using System;
using System.Collections.Generic;

namespace EmergentEchoes.Utilities.Internal
{
    public class Memorizer
    {
        public List<Event> LongTermMemory { get; private set; } = new();

        public LinkedList<Event> ShortTermMemory { get; private set; } = new();

        private readonly Dictionary<Actor, float> _relationships = new();

        public void AddRelationship(Actor actor, float value)
        {
            _relationships.Add(actor, Math.Clamp(value, -15, 15));
        }

        public bool IsLiked(Actor actor)
        {
            return _relationships[actor] > 10;
        }
    }
}