using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Internal;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    public class AssessAction : BaseAction
    {
        private readonly Crime _crime;
        private readonly bool _caseClosed;

        public AssessAction(ActorContext context, Crime crime, bool caseClosed = false)
            : base(context, ActionType.Assess)
        {
            _crime = crime;
            _caseClosed = caseClosed;
        }

        protected override void InitializeStates()
        {
            StateContext.StartingState = new AssessState(
                ActorContext,
                StateContext,
                _crime,
                _caseClosed
            );
        }
    }
}