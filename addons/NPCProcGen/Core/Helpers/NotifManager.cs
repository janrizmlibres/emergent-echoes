using System;

namespace NPCProcGen.Core.Helpers
{
    public class NotifManager
    {
        public event Action NavigationComplete;
        public event Action TheftComplete;

        public void NotifyNavigationComplete()
        {
            NavigationComplete?.Invoke();
        }

        public void NotifyTheftComplete()
        {
            TheftComplete?.Invoke();
        }
    }
}