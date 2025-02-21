using System;
using Godot;
using Godot.Collections;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Actions;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.States
{
    /// <summary>
    /// Represents the state of talking to another actor.
    /// </summary>
    public class TalkState : BaseState
    {
        private const int MinDuration = 10;
        private const int MaxDuration = 20;
        private const int MinCompanionshipIncrease = 20;
        private const int MaxCompanionshipIncrease = 40;

        private readonly float _companionshipIncrease;

        private readonly ActorTag2D _target;

        private float _duration;

        /// <summary>
        /// Initializes a new instance of the <see cref="TalkState"/> class.
        /// </summary>
        /// <param name="owner">The owner of the state.</param>
        public TalkState(ActorContext actorContext, StateContext stateContext, ActorTag2D target)
            : base(actorContext, stateContext, ActionState.Talk)
        {
            _target = target;
            _duration = GD.RandRange(MinDuration, MaxDuration);
            _companionshipIncrease = ComputeCompanionshipIncrease();
        }

        protected override EnterParameters GetEnterParameters()
        {
            return new EnterParameters
            {
                StateName = "TalkState",
                Data = new Array<Variant> { _target.GetParent<Node2D>() }
            };
        }

        protected override ExitParameters GetExitParameters()
        {
            return new ExitParameters
            {
                Data = new Array<Variant>
                {
                    _target.GetParent<Node2D>(),
                    _companionshipIncrease
                }
            };
        }

        protected override void ExecuteEnterLogic()
        {
            Array<Variant> data = new() { _target.GetParent<Node2D>() };

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
                ImproveCompanionship();
                _actorContext.Executor.FinishAction();
            }
        }

        private float ComputeCompanionshipIncrease()
        {
            float scaler = (_duration - MinDuration) / (MaxDuration - MinDuration);
            float increaseRange = MaxCompanionshipIncrease - MinCompanionshipIncrease;
            float amount = MinCompanionshipIncrease + scaler * increaseRange;
            return Math.Clamp(amount, MinCompanionshipIncrease, MaxCompanionshipIncrease);
        }

        private void ImproveCompanionship()
        {
            ResourceManager.Instance.ModifyResource(
                _actorContext.Actor,
                ResourceType.Companionship,
                _companionshipIncrease
            );
            ResourceManager.Instance.ModifyResource(
                _target,
                ResourceType.Companionship,
                _companionshipIncrease
            );

            _actorContext.Memorizer.UpdateRelationship(_target, 1);
            _target.Memorizer.UpdateRelationship(_actorContext.Actor, 1);
        }
    }
}