using System;
using Godot;

namespace NPCProcGen.Core.Helpers
{
    /// <summary>
    /// Provides common utility functions.
    /// </summary>
    public static class CommonUtils
    {
        /// <summary>
        /// Gets a shared instance of the random number generator.
        /// </summary>
        public static Random Rnd { get; private set; } = new Random();

        /// <summary>
        /// Gets a random position within a circular area.
        /// </summary>
        /// <param name="center">The center of the circular area.</param>
        /// <param name="radius">The radius of the circular area.</param>
        /// <param name="minRadius">The optional minimum radius.</param>
        /// <returns>A random position within the circular area.</returns>
        public static Vector2 GetRandomPosInCircularArea(Vector2 center, float radius, float? minRadius = null)
        {
            float angle = GD.Randf() * 2 * Mathf.Pi;
            float random_radius = minRadius != null ? Mathf.Lerp(minRadius.Value, radius, Mathf.Sqrt(GD.Randf()))
                : Mathf.Sqrt(GD.Randf()) * radius;

            return center + new Vector2(
                Mathf.Cos(angle) * random_radius,
                Mathf.Sin(angle) * random_radius
            );
        }

        public static void SetFacingDirectionsAndNotify(ActorTag2D _owner, ActorTag2D _target)
        {
            Vector2 directionToFace = _owner.Parent.GlobalPosition.DirectionTo(_target.Parent.GlobalPosition);
            _owner.EmitSignal(NPCAgent2D.SignalName.PetitionStarted, directionToFace);
            _target.EmitSignal(NPCAgent2D.SignalName.PetitionStarted, directionToFace * -1);
        }

        public static Vector2 GetInteractionPosition(ActorTag2D _owner, ActorTag2D _target)
        {
            Vector2 offset1 = new(15, 0);
            Vector2 adjustedPosition1 = _target.Parent.GlobalPosition + offset1;
            float distance1 = _owner.Parent.GlobalPosition.DistanceTo(adjustedPosition1);

            Vector2 offset2 = new(-15, 0);
            Vector2 adjustedPosition2 = _target.Parent.GlobalPosition + offset2;
            float distance2 = _owner.Parent.GlobalPosition.DistanceTo(adjustedPosition2);

            // Return the target's position adjusted by the best offset
            return distance1 < distance2 ? adjustedPosition1 : adjustedPosition2;
        }
    }
}