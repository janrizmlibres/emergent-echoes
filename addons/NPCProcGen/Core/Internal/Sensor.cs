using System.Collections.Generic;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.Internal
{
    // TODO: Make this class the only component that can access the world state
    /// <summary>
    /// The Sensor class is responsible for accessing the world state and retrieving actor information.
    /// </summary>
    public class Sensor
    {
        /// <summary>
        /// The world state instance.
        /// </summary>
        private readonly WorldState _worldState;

        /// <summary>
        /// Initializes a new instance of the Sensor class.
        /// </summary>
        public Sensor()
        {
            _worldState = WorldState.Instance;
        }

        /// <summary>
        /// Retrieves the list of actors from the world state.
        /// </summary>
        /// <returns>A read-only list of actors.</returns>
        public List<ActorTag2D> GetActors()
        {
            return _worldState.Actors;
        }

        public List<ActorTag2D> GetShuffledActors()
        {
            return CommonUtils.Shuffle(_worldState.Actors);
        }
    }
}