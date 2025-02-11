using System;
using Godot;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.States
{
    public class WaitState : BaseState, INavigationState
    {
        public const ActionState ActionStateValue = ActionState.Wait;

        private const float WaitDistance = 40;

        private readonly ActorTag2D _target;

        public event Action CompleteState;

        public WaitState(NPCAgent2D owner, ActionType action, ActorTag2D target) : base(owner, action)
        {
            _target = target;
        }

        public override void Subscribe()
        {
            _target.NotifManager.InteractionEnded += OnTargetInteractionEnded;
        }

        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} WaitState Enter");

            _owner.Sensor.SetTaskRecord(_actionType, ActionStateValue);

            CommonUtils.EmitSignal(
                _owner,
                NPCAgent2D.SignalName.ActionStateEntered,
                Variant.From(ActionStateValue)
            );
        }

        public override void Unsubscribe()
        {
            _target.NotifManager.InteractionEnded -= OnTargetInteractionEnded;
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
            Vector2 directionToInitiator = _target.Parent.GlobalPosition.DirectionTo(
                _owner.Parent.GlobalPosition
            );
            return _target.Parent.GlobalPosition + directionToInitiator * WaitDistance;
        }

        public bool OnNavigationComplete()
        {
            return true;
        }

        private void OnTargetInteractionEnded()
        {
            CompleteState?.Invoke();
        }
    }
}