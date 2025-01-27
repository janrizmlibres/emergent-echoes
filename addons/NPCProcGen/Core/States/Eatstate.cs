using System;
using Godot;
using NPCProcGen.Core.Components.Enums;

namespace NPCProcGen.Core.States
{
    public class EatState : BaseState
    {
        public event Action CompleteState;

        public EatState(NPCAgent2D owner) : base(owner) { }

        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} EatState Enter");
            _owner.NotifManager.ConsumptionComplete += OnConsumptionComplete;
            _owner.EmitSignal(NPCAgent2D.SignalName.ActionStateEntered, Variant.From(ActionState.Eat));
        }

        public override void Exit()
        {
            _owner.NotifManager.ConsumptionComplete -= OnConsumptionComplete;
            _owner.EmitSignal(NPCAgent2D.SignalName.ActionStateExited, Variant.From(ActionState.Eat));
        }

        private void OnConsumptionComplete(int foodConsumed)
        {
            if (foodConsumed > 0)
            {
                GD.Print($"{_owner.Parent.Name} consumed {foodConsumed} food");
                _owner.ConsumeFood(foodConsumed);
            }
            CompleteState?.Invoke();
        }
    }
}