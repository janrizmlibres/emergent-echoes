namespace NPCProcGen.Core.Events
{
    /// <summary>
    /// Represents a general event.
    /// </summary>
    public interface IGeneralEvent
    {
        /// <summary>
        /// Gets the local description of the event.
        /// </summary>
        /// <returns>A string describing the event locally.</returns>
        string GetLocalDescription();
    }

    /// <summary>
    /// Represents an event from a specific perspective.
    /// </summary>
    public interface IPerspectiveEvent
    {
        /// <summary>
        /// Gets the local description of the event.
        /// </summary>
        /// <param name="isDoer">Indicates if the description is for the doer.</param>
        /// <returns>A string describing the event locally.</returns>
        string GetLocalDescription(bool isDoer);
    }

    /// <summary>
    /// Represents an abstract base class for events.
    /// </summary>
    public abstract class Event
    {
        /// <summary>
        /// The actor who performed the event.
        /// </summary>
        protected readonly ActorTag2D _doer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        /// <param name="doer">The actor who performed the event.</param>
        public Event(ActorTag2D doer)
        {
            _doer = doer;
        }

        /// <summary>
        /// Gets the global description of the event.
        /// </summary>
        /// <returns>A string describing the event globally.</returns>
        public abstract string GetGlobalDescription();
    }
}