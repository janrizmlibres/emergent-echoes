using EmergentEchoes.addons.NPC2DNode;
using EmergentEchoes.Utilities.Components.Enums;
using Godot;
using System;

namespace EmergentEchoes.Utilities.Events
{
    public class PetitionRefusalEvent : Event, IPerspectiveEvent
    {
        private readonly NPC2D _deniedNPC;
        private readonly StatType _deniedResource;

        public PetitionRefusalEvent(NPC2D doer, NPC2D deniedNPC, StatType deniedResource) : base(doer)
        {
            _deniedNPC = deniedNPC;
            _deniedResource = deniedResource;
        }

        public override string GetGlobalDescription()
        {
            return $"{_doer.Name} refused a petition from {_deniedNPC.Name} for {_deniedResource}.";
        }

        public string GetLocalDescription(bool isDoer)
        {
            if (isDoer)
            {
                return $"Refused a petition from {_deniedNPC.Name} for {_deniedResource}.";
            }
            return $"{_doer.Name} refused a petition for {_deniedResource}.";
        }
    }
}