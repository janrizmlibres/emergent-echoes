using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using NPCProcGen.Core.Components;

namespace NPCProcGen.Core.Helpers
{
    /// <summary>
    /// Provides common utility functions.
    /// </summary>
    public static class CommonUtils
    {
        public const float PositionOffset = 12;

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
        public static Vector2 GetRandomPosInCircularArea(Vector2 center, float radius,
            float? minRadius = null)
        {
            float angle = GD.Randf() * 2 * Mathf.Pi;
            float random_radius = minRadius != null
                ? Mathf.Lerp(minRadius.Value, radius, Mathf.Sqrt(GD.Randf()))
                : Mathf.Sqrt(GD.Randf()) * radius;

            return center + new Vector2(
                Mathf.Cos(angle) * random_radius,
                Mathf.Sin(angle) * random_radius
            );
        }

        public static Vector2 GetInteractionPosition(ActorTag2D _owner, ActorTag2D _target)
        {
            Vector2 offset1 = new(PositionOffset, 0);
            Vector2 adjustedPosition1 = _target.Parent.GlobalPosition + offset1;
            float distance1 = _owner.Parent.GlobalPosition.DistanceTo(adjustedPosition1);

            Vector2 offset2 = new(-PositionOffset, 0);
            Vector2 adjustedPosition2 = _target.Parent.GlobalPosition + offset2;
            float distance2 = _owner.Parent.GlobalPosition.DistanceTo(adjustedPosition2);

            // Return the target's position adjusted by the best offset
            return distance1 < distance2 ? adjustedPosition1 : adjustedPosition2;
        }

        public static int CalculateSkewedAmount(ResourceStat resource, float minRange, float maxRange,
            float maxPossible)
        {
            // Calculate the deficiency for the NPC
            float deficiency = resource.LowerThreshold - resource.Amount;
            int minRaise = resource.GetMinRaise();

            // Introduce variability by adding a random factor
            double rndWeight = GD.Randf();
            double exponent = 1 + 5 * (1 - resource.Weight);
            float skewValue = (float)Math.Pow(rndWeight, exponent);

            float petitionMultiplier = minRange + (maxRange - minRange) * skewValue;
            float baseValue = Math.Max(deficiency, minRaise);

            float petitionAmount = baseValue * petitionMultiplier;
            // Ensure petition amount does not exceed the target's current resource amount
            petitionAmount = Math.Clamp(petitionAmount, minRaise, maxPossible);

            return (int)Math.Floor(petitionAmount);
        }

        public static void EmitSignal(ActorTag2D actor, StringName signalName, Variant? type = null,
            Array<Variant> data = null)
        {
            Error result = DelegateEmit();

            Error DelegateEmit()
            {
                if (data != null)
                {
                    DebugTool.Assert(type != null, "Variant parameter must not be null");
                    return actor.EmitSignal(signalName, type.Value, data);
                }

                if (type != null)
                {
                    return actor.EmitSignal(signalName, type.Value);
                }

                return actor.EmitSignal(signalName);
            }

            DebugTool.Assert(result != Error.Unavailable, "Signal parameters are invalid");
        }

        public static List<T> Shuffle<T>(List<T> list)
        {
            return list.OrderBy(_ => Rnd.Next()).ToList();
        }
    }
}