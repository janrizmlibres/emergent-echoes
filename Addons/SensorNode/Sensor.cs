using Godot;
using System;
using System.Collections.Generic;

namespace EmergentEchoes
{
    [Tool]
    public partial class Sensor : Node
    {
        private readonly WorldState.IWorldStateReader _worldState;

        public Sensor()
        {
            _worldState = WorldState.GetReader(this);
        }

        public List<CharacterBody2D> GetActors()
        {
            return _worldState.GetNPCList();
        }
    }
}