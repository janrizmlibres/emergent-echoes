using Godot;

namespace NPCProcGen.Core.Helpers
{
    public static class CommonUtils
    {
        public static Vector2 GetRandomFleePosition(Vector2 center, float radius)
        {
            float angle = GD.Randf() * 2 * Mathf.Pi;

            float random_radius = Mathf.Sqrt(GD.Randf()) * radius;

            return center + new Vector2(
                Mathf.Cos(angle) * random_radius,
                Mathf.Sin(angle) * random_radius
            );
        }
    }
}