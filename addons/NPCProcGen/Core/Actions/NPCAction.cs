using NPCProcGen.Core.States;
using Godot;
using System;

namespace NPCProcGen.Core.Actions
{
    public abstract class NPCAction
    {
        protected readonly NPCAgent2D _owner;
        protected ActionState _currentState;

        public NPCAction(NPCAgent2D owner)
        {
            _owner = owner;
        }

        protected void TransitionTo(ActionState newState)
        {
            _currentState = newState;
            _currentState.Enter();
        }

        protected void CompleteAction()
        {
            _owner.ReturnToIdle();
        }

        public void MoveToTarget(Vector2 target)
        {
            _owner.EmitSignal(NPCAgent2D.SignalName.MoveToTarget, target);
        }

        public abstract void Update();
    }
}