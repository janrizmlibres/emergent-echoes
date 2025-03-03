using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace EmergentEchoes.Utilities
{
    public static class CoreHelpers
    {
        public static IEnumerable<T> ShuffleEnum<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).OfType<T>().OrderBy(_ => GD.Randi());
        }

        public static List<T> ShuffleList<T>(List<T> list)
        {
            return list.OrderBy(_ => GD.Randi()).ToList();
        }
    }
}