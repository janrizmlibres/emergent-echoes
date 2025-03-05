using NPCProcGen.Core.States;
using Godot;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;

// ReSharper disable once CheckNamespace
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

        protected readonly ActorContext ActorContext;
        protected readonly StateContext StateContext;

        protected BaseAction(ActorContext context, ActionType actionType)
        {
            StateContext = new StateContext(this);
            ActorContext = context;
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
            ActorContext.Sensor.ClearTaskRecord();
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

            ActorContext.EmitSignal(
                NPCAgent2D.SignalName.ActionStarted,
                Variant.From(ActionType)
            );

            TransitionTo(StateContext.StartingState);
        }

        public void Finish()
        {
            ActorContext.EmitSignal(NPCAgent2D.SignalName.ActionEnded);
            TransitionTo(null);
            Terminate();
        }

        public bool IsInteractive()
        {
            return ActionType is ActionType.Petition or ActionType.Socialize
                or ActionType.Interrogate or ActionType.Interact;
        }

        protected virtual void Terminate() { }
        protected virtual void ExecuteRun() { }
        protected virtual void ExecuteUpdate(double delta) { }

        protected abstract void InitializeStates();
    }
}