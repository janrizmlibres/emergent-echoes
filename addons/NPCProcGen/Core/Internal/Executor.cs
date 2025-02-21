using System.Collections.Generic;
using Godot;
using NPCProcGen.Core.Actions;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Internal
{
    public class Executor
    {
        private readonly Stack<BaseAction> _actions = new();

        private readonly ActorContext _actorContext;

        public Executor(ActorContext context)
        {
            _actorContext = context;
        }

        public void Update(double delta)
        {
            if (_actions.TryPeek(out BaseAction action))
            {
                action.Update(delta);
            }
        }

        public void AddAction(BaseAction action)
        {
            DebugTool.Assert(action != null, "Action to be assigned cannot be null");

            if (_actions.TryPeek(out BaseAction currentAction))
            {
                currentAction.InterruptAction();
            }
            else
            {
                _actorContext.EmitSignal(NPCAgent2D.SignalName.ExecutionStarted);
            }

            _actions.Push(action);
            action.Run();
        }

        public void FinishAction()
        {
            DebugTool.Assert(_actions.Count > 0, "Actions cannot be null when completing an action");

            _actions.Pop().Finish();

            if (_actions.TryPeek(out BaseAction action))
            {
                action.Run();
            }
            else
            {
                _actorContext.GetNPCAgent2D().OnExecutionEnded();
                _actorContext.EmitSignal(NPCAgent2D.SignalName.ExecutionEnded);
            }
        }

        public void TerminateActions()
        {
            while (_actions.Count > 0)
            {
                _actions.Pop().InterruptAction();
            }
        }

        public Vector2 GetTargetPosition()
        {
            if (_actions.TryPeek(out BaseAction action))
            {
                BaseState currentState = action.CurrentState;
                return (currentState as INavigationState)?.GetTargetPosition()
                    ?? _actorContext.ActorNode2D.GlobalPosition;
            }

            return _actorContext.ActorNode2D.GlobalPosition;
        }

        public bool HasAction()
        {
            return _actions.Count > 0;
        }

        public bool IsNavigationRequired()
        {
            if (_actions.TryPeek(out BaseAction action))
            {
                BaseState currentState = action.CurrentState;
                return currentState is INavigationState state && state.IsNavigating();
            }

            return false;
        }

        public bool CompleteNavigation()
        {
            if (_actions.TryPeek(out BaseAction action))
            {
                BaseState currentState = action.CurrentState;
                return (currentState as INavigationState)?.OnNavigationComplete() ?? false; ;
            }
            return false;
        }

        public void CompleteConsumption()
        {
            if (_actions.TryPeek(out BaseAction action))
            {
                (action.CurrentState as EatState)?.OnConsumptionComplete();
            }
        }

        public void OnActorDetected(ActorTag2D actor)
        {
            if (_actions.TryPeek(out BaseAction action))
            {
                BaseState currentState = action.CurrentState;
                (currentState as IActorDetectionState)?.OnActorDetected(actor);
            }
        }
    }
}