using Godot;
using Godot.Collections;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Internal;

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
                Data = new Array<Variant>()
            };
        }

        protected override ExitParameters GetExitData()
        {
            return new ExitParameters
            {
                Data = new Array<Variant> { CommonUtils.DutyIncrease }
            };
        }

        protected override void ExecuteEnter()
        {
            Array<Variant> data = new() { _target.GetParent<Node2D>() };

            _actorContext.EmitSignal(
                ActorTag2D.SignalName.InteractionStarted,
                Variant.From((InteractionState)_actionState),
                data
            );

            data[0] = _actorContext.ActorNode2D;
            _target.TriggerInteraction(_actorContext.Actor, (InteractionState)_actionState, data);
            NotifManager.Instance.NotifyInteractionStarted(_actorContext.Actor);
        }

        protected override void ExecuteExit()
        {
            EvaluateParticipant();
            ResourceManager.Instance.ModifyResource(
                ResourceType.Companionship,
                CommonUtils.DutyIncrease,
                _actorContext.Actor
            );

            _actorContext.EmitSignal(ActorTag2D.SignalName.InteractionEnded);
            _target.StopInteraction();
            NotifManager.Instance.NotifyInteractionEnded(_actorContext.Actor);
        }

        public override void Update(double delta)
        {
            _duration -= (float)delta;

            if (_duration <= 0)
            {
                _actorContext.Executor.FinishAction();
            }
        }

        private void EvaluateParticipant()
        {
            float relationship = _actorContext.Memorizer.GetActorRelationship(_target);
            float successRate = ActorData.GetInterrogationProbability(relationship);
            _crime.ClearParticipant(_target, GD.Randf() <= successRate);
        }
    }
}