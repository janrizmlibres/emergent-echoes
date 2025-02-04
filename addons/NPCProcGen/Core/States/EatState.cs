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
            _owner.Sensor.SetTaskRecord(_owner, _actionType, ActionStateValue);

            Error result = _owner.EmitSignal(
                NPCAgent2D.SignalName.ActionStateEntered,
                Variant.From(ActionStateValue),
                data
            );
            DebugTool.Assert(result != Error.Unavailable, "Signal parameters are invalid");

            _owner.DeductFood(_amountToEat);
            // TODO: Move constant 10 elsewhere
            ResourceManager.Instance.ModifyResource(_owner, ResourceType.Satiation, _amountToEat * 10);
            CompleteState?.Invoke();
        }

        public override void Exit()
        {
            CommonUtils.EmitSignal(
                _owner,
                NPCAgent2D.SignalName.ActionStateExited,
                Variant.From(ActionStateValue),
                new Array<Variant>()
            );
        }

        private int ComputeFoodAmount()
        {
            ResourceStat resource = ResourceManager.Instance.GetResource(_owner, ResourceType.Satiation);
            float foodAmount = ResourceManager.Instance.GetResourceAmount(_owner, ResourceType.Food);
            float deficiency = resource.LowerThreshold - resource.Amount;
            return (int)Math.Min(Math.Ceiling(deficiency / 10), foodAmount);
        }
    }
}