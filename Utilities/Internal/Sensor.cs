using EmergentEchoes.Entities.Actors;
using Godot;
using System;
using System.Collections.Generic;

namespace EmergentEchoes.Utilities.Internal
{
    public partial class Sensor : Node
    {
        private WorldState _worldState;

        public Sensor()
        {
            _worldState = WorldState.Instance;
        }

        public List<Actor> GetActors()
        {
            return _worldState.GetActors();
        }
    }
}