using System;
using Godot;

namespace NPCProcGen.Core.Helpers
{
    public static class CommonUtils
    {
        public static Random Rnd { get; private set; } = new Random();

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
    }
}