using System;
using Godot;

namespace NPCProcGen.Core.Helpers
{
    /// <summary>
    /// Provides debugging tools and assertions.
    /// </summary>
    public static class DebugTool
    {
        /// <summary>
        /// Asserts a condition and throws an exception if the condition is false.
        /// </summary>
        /// <param name="condition">The condition to assert.</param>
        /// <param name="message">The message to display if the assertion fails.</param>
        public static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                GD.PrintErr(message);
                throw new ApplicationException($"Assert failed: {message}");
            }
        }
    }
}