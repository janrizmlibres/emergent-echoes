using System;
using Godot;
using NPCProcGen.Core.Actions;

namespace NPCProcGen.Core.States
{
    public class FleeState : ActionState, ILinearState
    {
        private static readonly float _fleeDistance = 1000;

        private Vector2 _target;

        public event Action OnComplete;

        public FleeState(NPCAgent2D owner) : base(owner) { }

        public override void Enter()
        {
            GD.Print("FleeState Enter");
            Vector2 fleePosition = GetRandomFleePosition(_owner.Parent.GlobalPosition);
            _target = fleePosition;
        }

        private static Vector2 GetRandomFleePosition(Vector2 center)
        {
            float angle = GD.Randf() * 2 * Mathf.Pi;

            // Get random radius (using square root for uniform distribution)
            float random_radius = Mathf.Sqrt(GD.Randf()) * _fleeDistance;

            // Convert polar coordinates to Cartesian
            return center + new Vector2(
                Mathf.Cos(angle) * random_radius,
                Mathf.Sin(angle) * random_radius
            );
        }

        public override void CompleteState()
        {
            OnComplete?.Invoke();
        }

        public override Vector2 GetTargetPosition()
        {
            return _target;
        }
    }
}