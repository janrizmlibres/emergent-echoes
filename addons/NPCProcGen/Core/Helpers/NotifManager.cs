using System;
using Godot;

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

        /// <summary>
        /// Event triggered when an actor is detected.
        /// </summary>
        public event Action<ActorTag2D> ActorDetected;

        /// <summary>
        /// Notifies subscribers that navigation is complete.
        /// </summary>
        public void NotifyNavigationComplete()
        {
            NavigationComplete?.Invoke();
        }

        /// <summary>
        /// Notifies subscribers that an actor has been detected.
        /// </summary>
        /// <param name="target">The detected actor.</param>
        public void NotifyActorDetected(ActorTag2D target)
        {
            ActorDetected?.Invoke(target);
        }
    }
}