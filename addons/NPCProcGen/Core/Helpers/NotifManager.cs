using System;
using Godot;

namespace NPCProcGen.Core.Helpers
{
    public class NotifManager
    {
        public event Action NavigationComplete;
        public event Action<ActorTag2D> ActorDetected;

        public void NotifyNavigationComplete()
        {
            NavigationComplete?.Invoke();
        }

        public void NotifyActorDetected(ActorTag2D target)
        {
            ActorDetected?.Invoke(target);
        }
    }
}