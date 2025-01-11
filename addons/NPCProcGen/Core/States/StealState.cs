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

        public event Action OnComplete;

        public StealState(NPCAgent2D owner, ActorTag2D target, ResourceType resourceType)
            : base(owner)
        {
            _target = target;
            _resourceType = resourceType;
        }

        public override void Enter()
        {
            GD.Print("StealState Enter");
        }

        public override void CompleteNavigation()
        {
            // TODO: Transfer resources and implement stealing
            _isTargetReached = true;
        }

        public override Vector2 GetTargetPosition()
        {
            return _target.Parent.GlobalPosition;
        }

        public override void CompleteState()
        {
            OnComplete?.Invoke();
        }

        public bool IsStealing()
        {
            return _isTargetReached;
        }
    }
}