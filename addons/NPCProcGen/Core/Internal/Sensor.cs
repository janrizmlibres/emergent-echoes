using System.Collections.Generic;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.Internal
{
    // TODO: Make this class the only component that can access the world state
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