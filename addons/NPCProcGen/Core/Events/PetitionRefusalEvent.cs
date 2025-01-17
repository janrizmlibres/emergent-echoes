using System.Security.AccessControl;

namespace NPCProcGen.Core.Events
{
    public class PetitionRefusalEvent : Event, IPerspectiveEvent
    {
        private readonly ActorTag2D _deniedNPC;
        private readonly ResourceType _deniedResource;

        public PetitionRefusalEvent(ActorTag2D doer, ActorTag2D deniedNPC, ResourceType deniedResource) : base(doer)
        {
            _deniedNPC = deniedNPC;
            _deniedResource = deniedResource;
        }

        // Global History
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