using System;
using System.Collections.Generic;
using EmergentEchoes.addons.NPC2DNode.Components;
using Godot;
using NPCProcGen.Core.Actions;

namespace NPCProcGen.Core.States
{
    public interface ILinearState
    {
        public event Action OnComplete;
    }

    public interface IBinaryState
    {
        public event Action<bool> OnComplete;
    }

    public interface INonlinearState
    {
        public event Action<Enum> OnComplete;
    }

    public abstract class ActionState
    {
        private readonly static List<Type> _navigationStates = new()
        {
            typeof(MoveState),
            typeof(FleeState)
        };

        protected readonly NPCAgent2D _owner;

        public ActionState(NPCAgent2D owner)
        {
            _owner = owner;
            // IsNavigationState = isNavigationState;
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
        public virtual void Update() { }

        public abstract Vector2 GetTargetPosition();
        public abstract void CompleteState();
    }
}