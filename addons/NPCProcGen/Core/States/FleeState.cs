using System;
using Godot;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.States
{
    public class FleeState : BaseState, INavigationState
    {
        private static readonly float _minDistance = 200;
        private static readonly float _maxDistance = 400;

        private Vector2 _target;

        public event Action CompleteState;

        public FleeState(NPCAgent2D owner) : base(owner) { }

        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} FleeState Enter");
            _owner.EmitSignal(NPCAgent2D.SignalName.ActionStateEntered, Variant.From(ActionState.Flee));
            _target = CommonUtils.GetRandomPosInCircularArea(_owner.Parent.GlobalPosition, _maxDistance, _minDistance);
            _owner.NotifManager.NavigationComplete += OnNavigationComplete;
        }

        public override void Exit()
        {
            _owner.EmitSignal(NPCAgent2D.SignalName.ActionStateExited, Variant.From(ActionState.Flee));
            _owner.NotifManager.NavigationComplete -= OnNavigationComplete;
        }

        public bool IsNavigating()
        {
            return true;
        }

        public Vector2 GetTargetPosition()
        {
            return _target;
        }

        private void OnNavigationComplete()
        {
            CompleteState?.Invoke();
        }
    }
}