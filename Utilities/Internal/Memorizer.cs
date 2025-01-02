using EmergentEchoes.Utilities.Events;
using Godot;
using System;
using System.Collections.Generic;

namespace EmergentEchoes.Utilities.Internal
{
    public class Memorizer
    {
        public List<Event> LongTermMemory { get; set; } = new();

        public LinkedList<Event> ShortTermMemory { get; set; } = new();
    }
}