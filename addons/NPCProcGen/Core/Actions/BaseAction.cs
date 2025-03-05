using NPCProcGen.Core.States;
using Godot;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;
using Godot.Collections;

namespace NPCProcGen.Core.Actions
{
    public interface ITargetedAction
    {
        public ActorTag2D GetTargetActor();
    }

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

        public void Interrupt()
        {
            _actorContext.Sensor.ClearTaskRecord();
            CurrentState?.Unsubscribe();
            CurrentState = null;
            Terminate();
        }

        public void Update(double delta)
        {
            CurrentState?.Update(delta);
            ExecuteUpdate(delta);
        }

        public void Run()
        {
            InitializeStates();
            ExecuteRun();

            _actorContext.EmitSignal(
                NPCAgent2D.SignalName.ActionStarted,
                Variant.From(ActionType)
            );

            TransitionTo(_stateContext.StartingState);
        }

        public void Finish()
        {
            _actorContext.EmitSignal(NPCAgent2D.SignalName.ActionEnded);
            TransitionTo(null);
            Terminate();
        }

        public virtual void Terminate() { }
        protected virtual void ExecuteRun() { }
        protected virtual void ExecuteUpdate(double delta) { }

        protected abstract void InitializeStates();
    }
}