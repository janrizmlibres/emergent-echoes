using EmergentEchoes.addons.NPC2DNode;
using Godot;
using System;

namespace EmergentEchoes.Utilities.Events
{
    public interface IGeneralEvent
    {
        string GetLocalDescription();
    }

    public interface IPerspectiveEvent
    {
        string GetLocalDescription(bool isDoer);
    }

    public abstract class Event
    {
        protected readonly NPC2D _doer;

        public Event(NPC2D doer)
        {
            _doer = doer;
        }

        public abstract string GetGlobalDescription();
    }
}