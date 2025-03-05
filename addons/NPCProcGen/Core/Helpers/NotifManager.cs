using System;
using NPCProcGen.Core.Components;

namespace NPCProcGen.Core.Helpers
{
    public sealed class NotifManager
    {
        private static readonly Lazy<NotifManager> _instance = new(() => new NotifManager());

        public static NotifManager Instance => _instance.Value;

        private NotifManager() { }

        public event Action<ActorTag2D, bool> PetitionAnswered;

        public event Action<ActorTag2D, ActorTag2D, Crime> CrimeCommitted;

        public event Action<ActorTag2D> InteractionStarted;
        public event Action<ActorTag2D> InteractionInterrupted;
        public event Action<ActorTag2D> InteractionEnded;

        public event Action<ActorTag2D, ActorTag2D> ActorDetained;
        public event Action<ActorTag2D> ActorCaptured;

        public void NotifyPetitionAnswered(ActorTag2D source, bool isAccepted)
        {
            PetitionAnswered?.Invoke(source, isAccepted);
        }

        public void NotifyCrimeCommitted(ActorTag2D source, ActorTag2D victim, Crime crime)
        {
            CrimeCommitted?.Invoke(source, victim, crime);
        }
        
        public void NotifyInterruptedInteraction(ActorTag2D source)
        {
            InteractionInterrupted?.Invoke(source);
        }

        public void NotifyInteractionStarted(ActorTag2D source)
        {
            InteractionStarted?.Invoke(source);
        }

        public void NotifyInteractionEnded(ActorTag2D source)
        {
            InteractionEnded?.Invoke(source);
        }

        public void NotifyActorDetained(ActorTag2D actor, ActorTag2D captor)
        {
            ActorDetained?.Invoke(actor, captor);
        }

        public void NotifyActorCaptured(ActorTag2D actor)
        {
            ActorCaptured?.Invoke(actor);
        }
    }
}