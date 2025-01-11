using NPCProcGen.Core.States;
using Godot;
using System;

namespace NPCProcGen.Core.Actions
{
    public abstract class NPCAction
    {
        protected readonly NPCAgent2D _owner;
        protected ActionState _currentState;

        public event Action OnComplete;

        public NPCAction(NPCAgent2D owner)
        {
            _owner = owner;
        }

        public void CompleteNavigation()
        {
            _currentState.CompleteNavigation();
        }

        public void CompleteState()
        {
            _currentState.CompleteState();
        }

        public bool HasNavigationState()
        {
            return _currentState.IsNavigationState();
        }

        public bool IsStealing()
        {
            return _currentState is StealState state && state.IsStealing();
        }

        public Vector2 GetTargetPosition()
        {
            return _currentState.GetTargetPosition();
        }

        protected void TransitionTo(ActionState newState)
        {
            _currentState = newState;
            _currentState.Enter();
        }

        protected void CompleteAction()
        {
            OnComplete?.Invoke();
        }

        public abstract void Update(double delta);
    }
}