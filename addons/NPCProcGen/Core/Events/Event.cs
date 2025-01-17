namespace NPCProcGen.Core.Events
{
    public interface IGeneralEvent
    {
        string GetLocalDescription();
    }

    public interface IPerspectiveEvent
    {
        string GetLocalDescription(bool isDoer);
    }

    public abstract class Event
    {
        protected readonly ActorTag2D _doer;

        public Event(ActorTag2D doer)
        {
            _doer = doer;
        }

        public abstract string GetGlobalDescription();
    }
}