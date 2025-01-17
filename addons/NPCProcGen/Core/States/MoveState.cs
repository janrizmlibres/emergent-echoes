using System;
using Godot;
using NPCProcGen.Core.Components.Enums;

namespace NPCProcGen.Core.States
{
    public class MoveState : BaseState, INavigationState
    {
        private readonly Node2D _target = null;
        private Vector2? _targetPosition = null;

        public event Action<bool> CompleteState;

        public MoveState(NPCAgent2D owner, Node2D target, Vector2? lastPos = null)
            : base(owner)
        {
            _target = target;
            _targetPosition = lastPos;
        }

        public MoveState(NPCAgent2D owner, Vector2 targetPosition)
            : base(owner)
        {
            _targetPosition = targetPosition;
        }

        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} MoveState Enter - Instance: {GetHashCode()}");
            _owner.EmitSignal(NPCAgent2D.SignalName.ActionStateEntered, Variant.From(ActionState.Move));
            _owner.NotifManager.NavigationComplete += OnNavigationComplete;
            _owner.NotifManager.ActorDetected += OnActorDetected;
        }

        public override void Exit()
        {
            _owner.EmitSignal(NPCAgent2D.SignalName.ActionStateExited, Variant.From(ActionState.Move));
            _owner.NotifManager.NavigationComplete -= OnNavigationComplete;
            _owner.NotifManager.ActorDetected -= OnActorDetected;
        }

        public bool IsNavigating()
        {
            return true;
        }

        public Vector2 GetTargetPosition()
        {
            return _targetPosition ?? _target.GlobalPosition;
        }

        private void OnNavigationComplete()
        {
            OnCompleteState(false);
        }

        private void OnActorDetected(ActorTag2D target)
        {
            if (target.Parent == _target)
            {
                OnCompleteState(true);
            }
        }

        private void OnCompleteState(bool isActorDetected)
        {
            CompleteState?.Invoke(isActorDetected);
        }
    }
}