using System.Security.AccessControl;

namespace NPCProcGen.Core.Events
{
    /// <summary>
    /// Represents an event where a petition is refused.
    /// </summary>
    public class PetitionRefusalEvent : Event, IPerspectiveEvent
    {
        private readonly ActorTag2D _deniedNPC;
        private readonly ResourceType _deniedResource;

        /// <summary>
        /// Initializes a new instance of the <see cref="PetitionRefusalEvent"/> class.
        /// </summary>
        /// <param name="doer">The actor who refused the petition.</param>
        /// <param name="deniedNPC">The actor whose petition was denied.</param>
        /// <param name="deniedResource">The resource that was denied.</param>
        public PetitionRefusalEvent(ActorTag2D doer, ActorTag2D deniedNPC, ResourceType deniedResource) : base(doer)
        {
            _deniedNPC = deniedNPC;
            _deniedResource = deniedResource;
        }

        /// <summary>
        /// Gets the global description of the event.
        /// </summary>
        /// <returns>A string describing the event globally.</returns>
        public override string GetGlobalDescription()
        {
            return $"{_doer.Name} refused a petition from {_deniedNPC.Name} for {_deniedResource}.";
        }

        /// <summary>
        /// Gets the local description of the event.
        /// </summary>
        /// <param name="isDoer">Indicates if the description is for the doer.</param>
        /// <returns>A string describing the event locally.</returns>
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