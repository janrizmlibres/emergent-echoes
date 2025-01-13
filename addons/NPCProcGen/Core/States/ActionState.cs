using System;
using System.Collections.Generic;
using Godot;

namespace NPCProcGen.Core.States
{
    public interface ILinearState
    {
        public event Action StateComplete;
    }

    public interface IBinaryState
    {
        public event Action<bool> StateComplete;
    }

    public interface INonlinearState
    {
        public event Action<Enum> StateComplete;
    }

    public abstract class ActionState
    {
        private readonly static List<Type> _navigationStates = new()
        {
            typeof(MoveState),
            typeof(WanderState),
            typeof(StealState),
            typeof(FleeState)
        };

        protected readonly NPCAgent2D _owner;

        public ActionState(NPCAgent2D owner)
        {
            _owner = owner;
        }

        public bool IsNavigationState()
        {
            if (_navigationStates.Contains(GetType()))
            {
                return true;
            }

            return false;
        }

        public virtual void Enter() { }
        public virtual void Update(double delta) { }

        public abstract Vector2 GetTargetPosition();
    }
}