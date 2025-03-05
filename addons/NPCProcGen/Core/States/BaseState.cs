using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;

// ReSharper disable once CheckNamespace
namespace NPCProcGen.Core.States
{
    public interface INavigationState
    {
        public bool IsNavigating();
        public Vector2 GetTargetPosition();
        public bool OnNavigationComplete();
    }

    public interface IActorDetectionState
    {
        public void OnActorDetected(ActorTag2D actor);
    }

    public class EnterParameters
    {
        public string StateName { get; set; }
        public Array<Variant> Data { get; set; }
    }

    public class ExitParameters
    {
        public Array<Variant> Data { get; init; }
    }

    public abstract class BaseState(ActorContext actorContext, StateContext stateContext, ActionState state)
    {
        protected readonly ActorContext ActorContext = actorContext;
        protected readonly StateContext StateContext = stateContext;

        protected readonly ActionState _actionState = state;

        public void Enter()
        {
            // TODO: Change to "Redirect"
            if (!Validate()) return;

            Subscribe();
            ExecuteEnter();

            EnterParameters enterParameters = GetEnterData();

            GD.Print($"{ActorContext.ActorNode2D.Name} {enterParameters.StateName} Enter");

            ActorContext.Sensor.SetTaskRecord(StateContext.Action.ActionType, _actionState);

            ActorContext.EmitSignal(
                NPCAgent2D.SignalName.StateEntered,
                Variant.From(_actionState),
                enterParameters.Data
            );
        }

        public void Exit()
        {
            Unsubscribe();
            ExecuteExit();

            ExitParameters exitParameters = GetExitData();

            ActorContext.Sensor.ClearTaskRecord();

            ActorContext.EmitSignal(
                NPCAgent2D.SignalName.StateExited,
                Variant.From(_actionState),
                exitParameters.Data
            );
        }

        public bool IsInteractive()
        {
            return _actionState is ActionState.Petition or ActionState.Talk
                or ActionState.Interact or ActionState.Interrogate;            
        }

        protected virtual void Subscribe() { }
        public virtual void Unsubscribe() { }

        public virtual void Update(double delta) { }

        protected virtual bool Validate() => true;

        protected virtual void ExecuteEnter() { }
        protected virtual void ExecuteExit() { }

        protected abstract EnterParameters GetEnterData();
        protected abstract ExitParameters GetExitData();
    }
}