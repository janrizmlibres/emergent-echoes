using System;
using Godot;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.States
{
    /// <summary>
    /// Represents the state of seeking a target.
    /// </summary>
    public class SeekState : BaseState, INavigationState
    {
        private const float SeekRadius = 400;

        private Vector2 _targetPosition;

        public event Action<NPCAgent2D> CompleteState;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeekState"/> class.
        /// </summary>
        /// <param name="owner">The owner of the state.</param>
        public SeekState(NPCAgent2D owner) : base(owner)
        {
            _targetPosition = CommonUtils.GetRandomPosInCircularArea(
                _owner.Parent.GlobalPosition, SeekRadius);
        }

        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} SeekState Enter");
            _owner.EmitSignal(NPCAgent2D.SignalName.ActionStateEntered, Variant.From(ActionState.Seek));
            _owner.NotifManager.NavigationComplete += OnNavigationComplete;
            _owner.NotifManager.ActorDetected += OnActorDetected;
        }

        public bool IsNavigating()
        {
            return true;
        }

        public Vector2 GetTargetPosition()
        {
            return _targetPosition;
        }

        private void OnNavigationComplete()
        {
            _targetPosition = CommonUtils.GetRandomPosInCircularArea(
                _owner.Parent.GlobalPosition, SeekRadius);
        }

        private void OnActorDetected(ActorTag2D actor)
        {
            if (actor is NPCAgent2D npc)
            {
                CompleteState?.Invoke(npc);
            }
        }
    }
}