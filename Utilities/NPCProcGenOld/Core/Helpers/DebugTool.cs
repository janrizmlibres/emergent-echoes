using System;
using Godot;

namespace NPCProcGen.Core.Helpers
{
    public static class DebugTool
    {
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