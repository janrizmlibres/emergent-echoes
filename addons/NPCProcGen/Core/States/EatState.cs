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

        protected override EnterParameters GetEnterData()
        {
            return new EnterParameters
            {
                StateName = "EatState",
                Data = new Array<Variant> { _amountToEat }
            };
        }

        protected override ExitParameters GetExitData()
        {
            int satiationIncrease = _amountToEat * CommonUtils.FoodSatiation;

            return new ExitParameters
            {
                Data = new Array<Variant>() { satiationIncrease }
            };
        }

        public void OnConsumptionComplete()
        {
            ResourceManager.Instance.ModifyResource(
                ResourceType.Satiation,
                _amountToEat * CommonUtils.FoodSatiation,
                ActorContext.Actor
            );

            ActorContext.Actor.DeductFood(_amountToEat);
            ActorContext.Executor.FinishAction();
        }

        private int ComputeFoodAmount()
        {
            ResourceManager resMgr = ResourceManager.Instance;

            ResourceStat resource = resMgr.GetResource(
                ResourceType.Satiation,
                ActorContext.Actor
            );

            float foodAmount = resMgr.GetResourceAmount(ResourceType.Food, ActorContext.Actor);
            float deficiency = resource.LowerThreshold - resource.Amount;

            DebugTool.Assert(foodAmount > 0, "Food amount must be greater than 0.");
            return (int)Math.Min(Math.Ceiling(deficiency / 10), foodAmount);
        }
    }
}