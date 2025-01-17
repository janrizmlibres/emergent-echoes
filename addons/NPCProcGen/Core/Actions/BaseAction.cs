using NPCProcGen.Core.States;
using Godot;
using System;
using NPCProcGen.Core.Components.Enums;

namespace NPCProcGen.Core.Actions
{
    public abstract class BaseAction
    {
        protected readonly NPCAgent2D _owner;
        protected BaseState _currentState;

        public event Action ActionComplete;

        public BaseAction(NPCAgent2D owner)
        {
            _owner = owner;
        }

        protected void TransitionTo(BaseState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState?.Enter();
        }

        protected void CompleteAction(ActionType actionType)
        {
            TransitionTo(null);
            ActionComplete?.Invoke();

            // TODO: Consider moving to owner itself
            _owner.EmitSignal(NPCAgent2D.SignalName.ExecutionEnded, Variant.From(actionType));
        }

        public Vector2 GetTargetPosition()
        {
            return (_currentState as INavigationState)?.GetTargetPosition() ?? _owner.Parent.GlobalPosition;
        }

        public bool IsNavigating()
        {
            return _currentState is INavigationState state && state.IsNavigating();
        }

        public Tuple<ResourceType, float> GetStolenResource()
        {
            return (_currentState as StealState)?.GetResourceToSteal() ?? null;
        }

        public abstract void Run();

        public abstract void Update(double delta);
    }
}