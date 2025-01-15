using System;
using Godot;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.States
{
    public class FleeState : ActionState, INavigationState
    {
        private static readonly float _fleeDistance = 400;

        private Vector2 _target;

        public event Action CompleteState;

        public FleeState(NPCAgent2D owner) : base(owner) { }

        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} FleeState Enter");
            _target = CommonUtils.GetRandomPosInCircularArea(_owner.Parent.GlobalPosition, _fleeDistance);
            _owner.NotifManager.NavigationComplete += OnNavigationComplete;
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
            _owner.NotifManager.NavigationComplete -= OnNavigationComplete;
            CompleteState?.Invoke();
        }
    }
}