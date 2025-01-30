using System;
using Godot;
using Godot.Collections;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;

namespace NPCProcGen.Core.States
{
    public class EatState : BaseState
    {
        private readonly int _amountToEat;

        public event Action CompleteState;

        public EatState(NPCAgent2D owner) : base(owner)
        {
            _amountToEat = ComputeFoodAmount();
        }

        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} EatState Enter");

            Array<Variant> data = new() { _amountToEat };
            _owner.EmitSignal(NPCAgent2D.SignalName.ActionStateEntered, Variant.From(ActionState.Eat), data);

            _owner.DeductFood(_amountToEat);
            // TODO: Move constant 10 elsewhere
            ResourceManager.Instance.ModifyResource(_owner, ResourceType.Satiation, _amountToEat * 10);
            CompleteState?.Invoke();
        }

        public override void Exit()
        {
            _owner.EmitSignal(NPCAgent2D.SignalName.ActionStateExited, Variant.From(ActionState.Eat));
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