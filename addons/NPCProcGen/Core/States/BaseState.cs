using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;

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
        public Array<Variant> Data { get; set; }
    }

    public abstract class BaseState
    {
        protected readonly ActorContext _actorContext;
        protected readonly StateContext _stateContext;

        protected readonly ActionState _actionState;

        public BaseState(ActorContext actorContext, StateContext stateContext, ActionState state)
        {
            _actorContext = actorContext;
            _stateContext = stateContext;

            _actionState = state;
        }

        public void Enter()
        {
            // TODO: Change to "Redirect"
            if (!Validate()) return;

            Subscribe();
            ExecuteEnter();

            EnterParameters enterParameters = GetEnterData();

            GD.Print($"{_actorContext.ActorNode2D.Name} {enterParameters.StateName} Enter");

            _actorContext.Sensor.SetTaskRecord(_stateContext.Action.ActionType, _actionState);

            _actorContext.EmitSignal(
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

            _actorContext.Sensor.ClearTaskRecord();

            _actorContext.EmitSignal(
                NPCAgent2D.SignalName.StateExited,
                Variant.From(_actionState),
                exitParameters.Data
            );
        }

        public virtual void Subscribe() { }
        public virtual void Unsubscribe() { }

        public virtual void Update(double delta) { }

        protected virtual bool Validate() => true;

        protected virtual void ExecuteEnter() { }
        protected virtual void ExecuteExit() { }

        protected abstract EnterParameters GetEnterData();
        protected abstract ExitParameters GetExitData();
    }
}