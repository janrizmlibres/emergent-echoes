using System;

namespace NPCProcGen.Core.Helpers
{
    public sealed class NotifManager
    {
        private static readonly Lazy<NotifManager> _instance = new(() => new NotifManager());

        public static NotifManager Instance => _instance.Value;

        private NotifManager() { }

        public event Action<ActorTag2D, bool> PetitionAnswered;

        public event Action<ActorTag2D> InteractionStarted;
        public event Action<ActorTag2D> InteractionEnded;

        public void NotifyPetitionAnswered(ActorTag2D source, bool isAccepted)
        {
            PetitionAnswered?.Invoke(source, isAccepted);
        }

        public void NotifyInteractionStarted(ActorTag2D source)
        {
            InteractionStarted?.Invoke(source);
        }

        public void NotifyInteractionEnded(ActorTag2D source)
        {
            InteractionEnded?.Invoke(source);
        }
    }
}