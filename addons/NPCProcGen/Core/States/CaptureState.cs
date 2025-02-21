using System.Linq;
using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Internal;
using NPCProcGen.Core.Traits;

namespace NPCProcGen.Core.States
{
    public class CaptureState : BaseState, INavigationState
    {
        private readonly ActorTag2D _criminal;
        private Vector2 _targetPosition;

        public CaptureState(ActorContext actorContext, StateContext stateContext,
            ActorTag2D criminal) : base(actorContext, stateContext, ActionState.Capture)
        {
            _criminal = criminal;
        }

        protected override void ExecuteEnterLogic()
        {
            PrisonArea2D prisonArea = _actorContext.Sensor.GetRandomPrison();
            DebugTool.Assert(prisonArea != null, "Prison area not found");
            _targetPosition = prisonArea.GetRandomPoint();

            // _criminal.Arrest();
            // _criminal.EmitSignal(ActorTag2D.SignalName.EventTriggered, Variant.From(EventType.Captured));
        }

        protected override EnterParameters GetEnterParameters()
        {
            return new EnterParameters
            {
                StateName = "CaptureState",
                Data = new Array<Variant> { _criminal }
            };
        }

        protected override ExitParameters GetExitParameters()
        {
            return new ExitParameters
            {
                Data = new Array<Variant> { _criminal }
            };
        }

        protected override void ExecuteExitLogic()
        {
            _actorContext.GetNPCAgent2D().Traits
                .OfType<LawfulTrait>()
                .FirstOrDefault()?
                .MarkCrimeAsSolved();

            // _criminal.EmitSignal(ActorTag2D.SignalName.EventTriggered, Variant.From(EventType.Released));
        }

        public bool IsNavigating()
        {
            return true;
        }

        public Vector2 GetTargetPosition()
        {
            return _targetPosition;
        }

        public bool OnNavigationComplete()
        {
            _actorContext.Executor.FinishAction();
            return true;
        }
    }
}