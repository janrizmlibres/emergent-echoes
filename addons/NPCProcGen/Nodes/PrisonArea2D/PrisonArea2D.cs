using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace NPCProcGen
{
    [Tool]
    public partial class PrisonArea2D : Area2D
    {
        private readonly List<CollisionShape2D> _collisionAreas = new();

        public override void _Ready()
        {
            _collisionAreas.AddRange(GetChildren()
                .OfType<CollisionShape2D>()
                .Where(collisionShape => collisionShape.Shape is RectangleShape2D)
            );
        }

        public Vector2 GetRandomPoint()
        {
            CollisionShape2D collisionShape = _collisionAreas
                .OrderBy(_ => GD.Randf())
                .FirstOrDefault();

            // Get the size of the rectangle
            Vector2 rectSize = (collisionShape.Shape as RectangleShape2D).Size;

            // Calculate half width and half height to center around (0,0)
            float halfWidth = rectSize.X / 2.0f;
            float halfHeight = rectSize.Y / 2.0f;

            // Generate random X and Y coordinates within the rectangle's bounds
            float randomX = GD.Randf() * rectSize.X - halfWidth;
            float randomY = GD.Randf() * rectSize.Y - halfHeight;

            // Return the random point as a Vector2
            return GlobalPosition + collisionShape.Position + new Vector2(randomX, randomY);
        }
    }
}