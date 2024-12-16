using Godot;
using System;
using System.Collections.Generic;

namespace EmergentEchoes
{
    [Tool]
    public partial class Sensor : Node
    {
        public static List<CharacterBody2D> GetActors()
        {
            return WorldState.Instance.NPCList;
        }
    }
}