using System;
using Godot;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.States
{
    public class SneakState : BaseState, INavigationState
    {
        public const ActionState ActionStateValue = ActionState.Sneak;

        private readonly ActorTag2D _target;

        public event Action CompleteState;

        public SneakState(NPCAgent2D owner, ActionType action, ActorTag2D target) : base(owner, action)
        {
            _target = target;
        }

        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} SneakState Enter");

            _owner.Sensor.SetTaskRecord(_actionType, ActionStateValue);

            CommonUtils.EmitSignal(
                _owner,
                NPCAgent2D.SignalName.ActionStateEntered,
                Variant.From(ActionStateValue)
            );
        }

        public override void Exit()
        {
            CommonUtils.EmitSignal(
                _owner,
                NPCAgent2D.SignalName.ActionStateExited,
                Variant.From(ActionStateValue)
            );
        }

        public bool IsNavigating()
        {
            return true;
        }

        public Vector2 GetTargetPosition()
        {
            return _target.GetRearPosition();
        }

        public bool OnNavigationComplete()
        {
            CompleteState?.Invoke();
            return true;
        }
    }
}