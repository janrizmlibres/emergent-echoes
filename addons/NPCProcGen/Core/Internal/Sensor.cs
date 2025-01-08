using System.Collections.Generic;

namespace NPCProcGen.Core.Internal
{
    public class Sensor
    {
        private readonly WorldState _worldState;

        public Sensor()
        {
            _worldState = WorldState.Instance;
        }

        public IReadOnlyList<ActorTag2D> GetActors()
        {
            return _worldState.Actors;
        }
    }
}