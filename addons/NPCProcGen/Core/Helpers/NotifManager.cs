using System;

namespace NPCProcGen.Core.Helpers
{
    /// <summary>
    /// Manages notifications for navigation completion and actor detection.
    /// </summary>
    public class NotifManager
    {
        public event Action<bool> PetitionAnswered;

        public event Action InteractionStarted;
        public event Action InteractionEnded;

        public event Action ActorImprisoned;

        public void NotifyPetitionAnswered(bool isAccepted)
        {
            PetitionAnswered?.Invoke(isAccepted);
        }

        public void NotifyInteractionStarted()
        {
            InteractionStarted?.Invoke();
        }

        public void NotifyInteractionEnded()
        {
            InteractionEnded?.Invoke();
        }

        public void NotifyActorImprisoned()
        {
            ActorImprisoned?.Invoke();
        }
    }
}