using System;
using System.Collections.Generic;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Components.Enums;

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
        private static readonly WorldState _worldState = new();

        public static void InitializeWorldState(List<ActorTag2D> actors)
        {
            _worldState.Initialize(actors);
        }

        /// <summary>
        /// Retrieves the list of actors from the world state.
        /// </summary>
        /// <returns>A read-only list of actors.</returns>
        public List<ActorTag2D> GetActors()
        {
            return _worldState.Actors;
        }

        public Tuple<ActionType, ActionState> GetTaskRecord(ActorTag2D actor)
        {
            return _worldState.GetTaskRecord(actor);
        }

        public void SetTaskRecord(ActorTag2D actor, ActionType actionType, ActionState actionState)
        {
            _worldState.SetTaskRecord(actor, actionType, actionState);
        }

        public void ResetTaskRecord(ActorTag2D actor)
        {
            _worldState.ResetTaskRecord(actor);
        }

        public bool IsActorBusy(ActorTag2D actor)
        {
            return _worldState.IsActorBusy(actor);
        }
    }
}