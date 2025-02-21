using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.States
{
    public class InterrogateState : BaseState
    {
        private const int MinDuration = 10;
        private const int MaxDuration = 20;

        private readonly Crime _crime;
        private readonly ActorTag2D _target;
        private float _duration;

        public InterrogateState(ActorContext actorContext, StateContext stateContext,
            Crime crime, ActorTag2D target)
            : base(actorContext, stateContext, ActionState.Interrogate)
        {
            _crime = crime;
            _target = target;
            _duration = GD.RandRange(MinDuration, MaxDuration);
        }

        protected override EnterParameters GetEnterParameters()
        {
            return new EnterParameters
            {
                StateName = "InterrogateState",
                Data = new Array<Variant> { _target.GetParent<Node2D>() }
            };
        }

        protected override ExitParameters GetExitParameters()
        {
            return new ExitParameters
            {
                Data = new Array<Variant> { _target.GetParent<Node2D>() }
            };
        }

        protected override void ExecuteEnterLogic()
        {
            Array<Variant> data = new() { _actorContext.ActorNode2D };

            _target.TriggerInteraction(_actorContext.Actor, _actionState, data);
            NotifManager.Instance.NotifyInteractionStarted(_actorContext.Actor);
        }

        protected override void ExecuteExitLogic()
        {
            _target.StopInteraction();
            NotifManager.Instance.NotifyInteractionEnded(_actorContext.Actor);
        }

        public override void Update(double delta)
        {
            _duration -= (float)delta;

            if (_duration <= 0)
            {
                CalculateSuccess();
                _actorContext.Executor.FinishAction();
            }
        }

        private void CalculateSuccess()
        {
            float relationship = _actorContext.Memorizer.GetActorRelationship(_target);
            float successRate = ActorData.GetInterrogationProbability(relationship);

            if (GD.Randf() <= successRate)
            {
                _crime.MarkSuccessfulWitness(_target);
            }
            else
            {
                _crime.MarkFailedWitness(_target);
            }
        }
    }
}