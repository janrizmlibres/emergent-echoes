using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Internal;
using NPCProcGen.Core.Traits;

namespace NPCProcGen.Core.States
{
    public class InterrogateState : BaseState
    {
        private readonly ActorTag2D _target;
        private readonly Crime _crime;

        private float _duration = 15;

        public InterrogateState(ActorContext actorContext, StateContext stateContext,
            ActorTag2D target, Crime crime)
            : base(actorContext, stateContext, ActionState.Interrogate)
        {
            _target = target;
            _crime = crime;
        }

        protected override EnterParameters GetEnterData()
        {
            return new EnterParameters
            {
                StateName = "InterrogateState",
                Data = new Array<Variant> { _target.GetParent<Node2D>() }
            };
        }

        protected override ExitParameters GetExitData()
        {
            return new ExitParameters
            {
                Data = new Array<Variant> { _target.GetParent<Node2D>() }
            };
        }

        protected override void ExecuteEnter()
        {
            Array<Variant> data = new() { _actorContext.ActorNode2D };

            _target.TriggerInteraction(_actorContext.Actor, _actionState, data);
            NotifManager.Instance.NotifyInteractionStarted(_actorContext.Actor);
        }

        protected override void ExecuteExit()
        {
            _target.StopInteraction();
            NotifManager.Instance.NotifyInteractionEnded(_actorContext.Actor);
        }

        public override void Update(double delta)
        {
            _duration -= (float)delta;

            if (_duration <= 0)
            {
                ClearParticipant();
                CheckInvestigationStatus();
                _actorContext.Executor.FinishAction();
            }
        }

        private void ClearParticipant()
        {
            float relationship = _actorContext.Memorizer.GetActorRelationship(_target);
            float successRate = ActorData.GetInterrogationProbability(relationship);
            _crime.ClearParticipant(_target, GD.Randf() <= successRate);
        }

        private void CheckInvestigationStatus()
        {
            if (_crime.IsDeposed() && !_crime.IsUnsolvable())
            {
                _actorContext.LawfulModule.ClearCase(CrimeStatus.Unsolved);
            }
        }
    }
}