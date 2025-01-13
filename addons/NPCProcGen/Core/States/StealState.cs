using System;
using Godot;
using NPCProcGen.Core.Components.Enums;

namespace NPCProcGen.Core.States
{
    public class StealState : ActionState, ILinearState
    {
        private readonly ActorTag2D _target;
        private readonly ResourceType _resourceType;

        private bool _isTargetReached = false;

        public event Action StateComplete;

        public StealState(NPCAgent2D owner, ActorTag2D target, ResourceType resourceType)
            : base(owner)
        {
            _target = target;
            _resourceType = resourceType;
        }

        public override void Enter()
        {
            GD.Print("StealState Enter");
            _owner.NotifManager.NavigationComplete += OnNavigationComplete;
            _owner.NotifManager.TheftComplete += OnTheftComplete;
        }

        public override Vector2 GetTargetPosition()
        {
            return _target.Parent.GlobalPosition;
        }

        public bool IsStealing()
        {
            return _isTargetReached;
        }

        private void OnNavigationComplete()
        {
            // TODO: Transfer resources and implement stealing
            _isTargetReached = true;
        }

        private void OnTheftComplete()
        {
            StateComplete?.Invoke();
        }
    }
}