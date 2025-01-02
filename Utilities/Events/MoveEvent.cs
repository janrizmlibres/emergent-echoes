using EmergentEchoes.addons.NPC2DNode;
using Godot;
using System;

namespace EmergentEchoes.Utilities.Events
{
    public class MoveEvent : Event, IGeneralEvent
    {
        private readonly Marker2D _destination;

        public MoveEvent(NPC2D doer, Marker2D destination) : base(doer)
        {
            _destination = destination;
        }

        public override string GetGlobalDescription()
        {
            return $"{_doer.Name} moved to {_destination}.";
        }

        public string GetLocalDescription()
        {
            return $"Moved to {_destination}.";
        }
    }
}