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

        public Dictionary<Actor, float> Relationships { get; set; }

        public bool IsFriendly(Actor actor)
        {
            return Relationships[actor] > 5;
        }

        public bool IsTrusted(Actor actor)
        {
            return Relationships[actor] > 10;
        }

        public bool IsClose(Actor actor)
        {
            return Relationships[actor] > 15;
        }
    }
}