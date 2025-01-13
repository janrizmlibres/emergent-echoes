using System;

namespace NPCProcGen.Core.Helpers
{
    public class NotifManager
    {
        public event Action NavigationComplete;
        public event Action TheftComplete;
        public event Action<ActorTag2D> ActorDetected;

        public void NotifyNavigationComplete()
        {
            NavigationComplete?.Invoke();
        }

        public void NotifyTheftComplete()
        {
            TheftComplete?.Invoke();
        }

        public void NotifyActorDetected(ActorTag2D actor)
        {
            ActorDetected?.Invoke(actor);
        }
    }
}