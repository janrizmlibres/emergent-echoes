using System;
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
            NotifManager.Instance.ActorDetained += OnActorDetained;
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
                currentAction.Interrupt();
            }
            else
            {
                _actorContext.EmitSignal(NPCAgent2D.SignalName.ExecutionStarted);
            }

            _actions.Push(action);
            action.Run();
        }

        public void TerminateAction()
        {
            _actions.Pop().Interrupt();
            AttemptResume();
        }

        public void FinishAction()
        {
            DebugTool.Assert(_actions.Count > 0, "Actions cannot be null when completing an action");
            _actions.Pop().Finish();
            AttemptResume();
        }

        private void AttemptResume()
        {
            if (_actions.TryPeek(out BaseAction action))
            {
                action.Run();
            }
            else
            {
                _actorContext.GetNPCAgent2D().StartEvaluationTimer();
                _actorContext.EmitSignal(NPCAgent2D.SignalName.ExecutionEnded);
            }
        }

        public void TerminateExecution()
        {
            while (_actions.Count > 0)
            {
                _actions.Pop().Interrupt();
            }

            _actorContext.EmitSignal(NPCAgent2D.SignalName.ExecutionEnded);
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
                return (currentState as INavigationState)?.OnNavigationComplete() ?? false;
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

        public void CompletePlanting()
        {
            if (_actions.TryPeek(out BaseAction action))
            {
                (action.CurrentState as PlantState)?.OnPlantingComplete();
            }
        }

        public void CompleteHarvest()
        {
            if (_actions.TryPeek(out BaseAction action))
            {
                (action.CurrentState as HarvestState)?.OnHarvestComplete();
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

        private void OnActorDetained(ActorTag2D actor)
        {
            if (actor == null) throw new ArgumentNullException(nameof(actor));

            if (_actions.TryPeek(out BaseAction action))
            {
                if ((action as ITargetedAction)?.GetTargetActor() != actor) return;
                TerminateAction();
            }
        }
    }
}