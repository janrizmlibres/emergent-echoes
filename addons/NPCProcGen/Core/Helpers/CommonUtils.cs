using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using NPCProcGen.Core.Components;

namespace NPCProcGen.Core.Helpers
{
    public static class CommonUtils
    {
        public const float PositionOffset = 12;
        public const int FoodSatiation = 10;

        public static List<T> Shuffle<T>(List<T> list)
        {
            return list.OrderBy(_ => GD.Randi()).ToList();
        }

        public static void EmitSignal(ActorTag2D actor, StringName signalName, params Variant[] args)
        {
            Error result = actor.EmitSignal(signalName, args);
            DebugTool.Assert(result != Error.Unavailable, "Signal emitted error");
        }

        public static Vector2 GetOmnidirectionalWaypoint(Vector2 origin, Vector2 target)
        {
            Vector2 directionToOrigin = target.DirectionTo(origin);
            return target + directionToOrigin * PositionOffset;
        }

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
            petitionAmount = Math.Min(petitionAmount, maxPossible);
            petitionAmount = Math.Max(petitionAmount, minRaise);

            return (int)Math.Floor(petitionAmount);
        }
    }
}