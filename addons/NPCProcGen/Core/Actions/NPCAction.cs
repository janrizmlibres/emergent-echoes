using NPCProcGen.Core.States;
using Godot;
using System;

namespace NPCProcGen.Core.Actions
{
    public abstract class NPCAction
    {
        protected readonly NPCAgent2D _owner;
        protected ActionState _currentState;

        public event Action ActionComplete;

        public NPCAction(NPCAgent2D owner)
        {
            _owner = owner;
        }

        protected void TransitionTo(ActionState newState)
        {
            _currentState = newState;
            _currentState.Enter();
        }

        protected void OnActionComplete()
        {
            ActionComplete?.Invoke();
        }

        public Vector2 GetTargetPosition()
        {
            return _currentState.GetTargetPosition();
        }

        public bool HasNavigationState()
        {
            return _currentState.IsNavigationState();
        }

        public bool IsStealing()
        {
            return _currentState is StealState state && state.IsStealing();
        }

        public abstract void Update(double delta);
    }
}