using System;

namespace NPCProcGen.Core.Helpers
{
    /// <summary>
    /// Manages notifications for navigation completion and actor detection.
    /// </summary>
    public class NotifManager
    {
        /// <summary>
        /// Event triggered when navigation is complete.
        /// </summary>
        public event Action NavigationComplete;
        public event Action<bool> PetitionAnswered;
        public event Action ConsumptionComplete;

        /// <summary>
        /// Event triggered when an actor is detected.
        /// </summary>
        public event Action<ActorTag2D> ActorDetected;

        public event Action InteractionStarted;
        public event Action InteractionEnded;

        /// <summary>
        /// Notifies subscribers that navigation is complete.
        /// </summary>
        public void NotifyNavigationComplete()
        {
            NavigationComplete?.Invoke();
        }

        public void NotifyPetitionAnswered(bool isAccepted)
        {
            PetitionAnswered?.Invoke(isAccepted);
        }

        public void NotifyConsumptionComplete()
        {
            ConsumptionComplete?.Invoke();
        }

        /// <summary>
        /// Notifies subscribers that an actor has been detected.
        /// </summary>
        /// <param name="target">The detected actor.</param>
        public void NotifyActorDetected(ActorTag2D target)
        {
            ActorDetected?.Invoke(target);
        }

        public void NotifyInteractionStarted()
        {
            InteractionStarted?.Invoke();
        }

        public void NotifyInteractionEnded()
        {
            InteractionEnded?.Invoke();
        }
    }
}