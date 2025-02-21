using NPCProcGen.Core.States;
using Godot;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.Actions
{
    public abstract class BaseAction
    {
        public ActionType ActionType { get; private set; }
        public BaseState CurrentState { get; private set; }

        protected readonly ActorContext _actorContext;
        protected readonly StateContext _stateContext;

        public BaseAction(ActorContext context, ActionType actionType)
        {
            _stateContext = new(this);
            _actorContext = context;
            ActionType = actionType;
        }

        public void TransitionTo(BaseState newState)
        {
            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState?.Enter();
        }

        public void InterruptAction()
        {
            _actorContext.Sensor.ClearTaskRecord();
            CurrentState?.Unsubscribe();
            CurrentState = null;
            Terminate();
        }

        public void Update(double delta)
        {
            CurrentState?.Update(delta);
        }

        public void Run()
        {
            InitializeStates();
            ExecuteRun();

            _actorContext.EmitSignal(
                NPCAgent2D.SignalName.ActionStarted,
                Variant.From(ActionType)
            );

            BaseState startState = GetStartingState();
            TransitionTo(startState);
        }

        public void Finish()
        {
            _actorContext.EmitSignal(
                NPCAgent2D.SignalName.ActionEnded,
                Variant.From(ActionType)
            );

            TransitionTo(null);
            Terminate();
        }

        public virtual void Terminate() { }

        protected virtual void ExecuteRun() { }

        protected abstract void InitializeStates();
        protected abstract BaseState GetStartingState();
    }
}