using NPCProcGen.Core.Actions;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Internal
{
    public class StateContext
    {
        public BaseAction Action { get; private set; }

        public WanderState WanderState { get; set; }
        public WaitState WaitState { get; set; }
        public FleeState FleeState { get; set; }

        public BaseState ApproachState { get; set; }
        public BaseState ContactState { get; set; }

        public StateContext(BaseAction action)
        {
            Action = action;
        }

        public void ApproachTarget(ActorTag2D target)
        {
            if (WaitState != null)
            {
                BaseState nextState = target.Sensor.IsBusy() ? WaitState : ApproachState;
                Action.TransitionTo(nextState);
            }
            else
            {
                Action.TransitionTo(ApproachState);
            }
        }
    }
}