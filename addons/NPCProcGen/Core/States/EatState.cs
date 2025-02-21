using System;
using Godot;
using Godot.Collections;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.States
{
    public class EatState : BaseState
    {
        private readonly int _amountToEat;

        public EatState(ActorContext actorContext, StateContext stateContext)
            : base(actorContext, stateContext, ActionState.Eat)
        {
            _amountToEat = ComputeFoodAmount();
        }

        protected override EnterParameters GetEnterParameters()
        {
            return new EnterParameters
            {
                StateName = "EatState",
                Data = new Array<Variant> { _amountToEat }
            };
        }

        protected override ExitParameters GetExitParameters()
        {
            // ! Magic number 10
            int satiationIncrease = _amountToEat * 10;

            return new ExitParameters
            {
                Data = new Array<Variant>() { satiationIncrease }
            };
        }

        public void OnConsumptionComplete()
        {
            _actorContext.Actor.DeductFood(_amountToEat);
            // TODO: Move constant 10 elsewhere
            ResourceManager.Instance.ModifyResource(_actorContext.Actor, ResourceType.Satiation, _amountToEat * 10);

            _actorContext.Executor.FinishAction();
        }

        private int ComputeFoodAmount()
        {
            ResourceStat resource = ResourceManager.Instance.GetResource(_actorContext.Actor, ResourceType.Satiation);
            float foodAmount = ResourceManager.Instance.GetResourceAmount(_actorContext.Actor, ResourceType.Food);
            float deficiency = resource.LowerThreshold - resource.Amount;

            DebugTool.Assert(foodAmount > 0, "Food amount must be greater than 0.");
            return (int)Math.Min(Math.Ceiling(deficiency / 10), foodAmount);
        }
    }
}