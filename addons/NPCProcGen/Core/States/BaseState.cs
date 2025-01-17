using Godot;

namespace NPCProcGen.Core.States
{
    public interface INavigationState
    {
        public bool IsNavigating();
        public Vector2 GetTargetPosition();
    }

    public abstract class BaseState
    {
        protected readonly NPCAgent2D _owner;

        public BaseState(NPCAgent2D owner)
        {
            _owner = owner;
        }

        public virtual void Enter() { }
        public virtual void Update(double delta) { }
        public virtual void Exit() { }
    }
}