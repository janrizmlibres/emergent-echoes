using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Internal;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    public class InterrogateAction : BaseAction, ITargetedAction
    {
        private readonly ActorTag2D _target;
        private readonly Crime _crime;

        public InterrogateAction(ActorContext context, ActorTag2D target, Crime crime)
            : base(context, ActionType.Interrogate)
        {
            _target = target;
            _crime = crime;
        }

        protected override void InitializeStates()
        {
            StateContext.StartingState = new SearchState(ActorContext, StateContext, _target);
            StateContext.WanderState = new(ActorContext, StateContext, _target);
            StateContext.ApproachState = new EngageState(ActorContext, StateContext, _target,
                Waypoint.Lateral);
            StateContext.WaitState = new(ActorContext, StateContext, _target);
            StateContext.ContactState = new InterrogateState(ActorContext, StateContext, _target,
                _crime);
        }

        public ActorTag2D GetTargetActor() => _target;
    }
}