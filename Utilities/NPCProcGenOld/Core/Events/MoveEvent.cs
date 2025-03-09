using Godot;

namespace NPCProcGen.Core.Events
{
    /// <summary>
    /// Represents an event where an actor moves to a destination.
    /// </summary>
    public class MoveEvent : Event, IGeneralEvent
    {
        private readonly Marker2D _destination;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveEvent"/> class.
        /// </summary>
        /// <param name="doer">The actor who moved.</param>
        /// <param name="destination">The destination marker.</param>
        public MoveEvent(ActorTag2D doer, Marker2D destination) : base(doer)
        {
            _destination = destination;
        }

        /// <summary>
        /// Gets the global description of the event.
        /// </summary>
        /// <returns>A string describing the event globally.</returns>
        public override string GetGlobalDescription()
        {
            return $"{_doer.Name} moved to {_destination}.";
        }

        /// <summary>
        /// Gets the local description of the event.
        /// </summary>
        /// <returns>A string describing the event locally.</returns>
        public string GetLocalDescription()
        {
            return $"Moved to {_destination}.";
        }
    }
}