using System;
using Godot;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.States
{
    public class FleeState : ActionState, ILinearState
    {
        private static readonly float _fleeDistance = 400;

        private Vector2 _target;

        public event Action StateComplete;

        public FleeState(NPCAgent2D owner) : base(owner) { }

        public override void Enter()
        {
            GD.Print("FleeState Enter");
            _target = CommonUtils.GetRandomFleePosition(_owner.Parent.GlobalPosition, _fleeDistance);
            _owner.NotifManager.NavigationComplete += OnNavigationComplete;
        }

        public override Vector2 GetTargetPosition()
        {
            return _target;
        }

        private void OnNavigationComplete()
        {
            StateComplete?.Invoke();
        }
    }
}