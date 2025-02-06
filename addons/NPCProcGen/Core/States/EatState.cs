using System;
using Godot;
using Godot.Collections;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.States
{
    public class EatState : BaseState
    {
        public const ActionState ActionStateValue = ActionState.Eat;

        private readonly int _amountToEat;

        public event Action CompleteState;

        public EatState(NPCAgent2D owner, ActionType action) : base(owner, action)
        {
            _amountToEat = ComputeFoodAmount();
        }

        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} EatState Enter");

            Array<Variant> data = new() { _amountToEat };
            _owner.Sensor.SetTaskRecord(_actionType, ActionStateValue);

            _owner.NotifManager.ConsumptionComplete += OnConsumptionComplete;
            CommonUtils.EmitSignal(
                _owner,
                NPCAgent2D.SignalName.ActionStateEntered,
                Variant.From(ActionStateValue),
                data
            );
        }

        public override void Exit()
        {
            GD.Print($"{_owner.Parent.Name} EatState Exit");
            _owner.NotifManager.ConsumptionComplete -= OnConsumptionComplete;

            // ! Magic number 10
            int satiationIncrease = _amountToEat * 10;
            Array<Variant> data = new() { satiationIncrease };

            CommonUtils.EmitSignal(
                _owner,
                NPCAgent2D.SignalName.ActionStateExited,
                Variant.From(ActionStateValue),
                data
            );
        }

        private void OnConsumptionComplete()
        {
            _owner.DeductFood(_amountToEat);
            // TODO: Move constant 10 elsewhere
            ResourceManager.Instance.ModifyResource(_owner, ResourceType.Satiation, _amountToEat * 10);
            CompleteState?.Invoke();
        }

        private int ComputeFoodAmount()
        {
            ResourceStat resource = ResourceManager.Instance.GetResource(_owner, ResourceType.Satiation);
            float foodAmount = ResourceManager.Instance.GetResourceAmount(_owner, ResourceType.Food);
            float deficiency = resource.LowerThreshold - resource.Amount;

            DebugTool.Assert(foodAmount > 0, "Food amount must be greater than 0.");
            return (int)Math.Min(Math.Ceiling(deficiency / 10), foodAmount);
        }
    }
}