using System;
using System.Collections.Generic;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
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
        private static readonly WorldState _worldState = new();
        private readonly ActorTag2D _owner;

        public Sensor(ActorTag2D owner)
        {
            _owner = owner;
        }

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

        public Tuple<ActionType, ActionState> GetTaskRecord()
        {
            return _worldState.GetTaskRecord(_owner);
        }

        public ActionType? GetTaskActionType()
        {
            return _worldState.GetTaskRecord(_owner)?.Item1;
        }

        public ActionState? GetTaskActionState()
        {
            return _worldState.GetTaskRecord(_owner)?.Item2;
        }

        public void SetTaskRecord(ActionType actionType, ActionState actionState)
        {
            _worldState.SetTaskRecord(_owner, actionType, actionState);
        }

        public void ClearTaskRecord()
        {
            _worldState.ResetTaskRecord(_owner);
        }

        public bool IsActorBusy()
        {
            return _worldState.IsActorBusy(_owner);
        }

        public bool IsActorPetitioning()
        {
            return _worldState.GetTaskRecord(_owner)?.Item1 == ActionType.Petition;
        }

        public ResourceType? GetPetitionResourceType()
        {
            return _worldState.GetPetitionResourceType(_owner);
        }

        public void SetPetitionResourceType(ResourceType type)
        {
            _worldState.SetPetitionResourceType(_owner, type);
        }

        public void ClearPetitionResourceType()
        {
            _worldState.ClearPetitionResourceType(_owner);
        }

        public bool HasCrimes()
        {
            return _worldState.HasCrimes();
        }

        public void RecordCrime(Crime crime)
        {
            _worldState.RecordCrime(crime);
        }

        public Crime InvestigateCrime()
        {
            DebugTool.Assert(_owner is NPCAgent2D, "Only NPC agents can investigate crimes");
            return _worldState.InvestigateCrime(_owner as NPCAgent2D);
        }

        public void CloseInvestigation()
        {
            DebugTool.Assert(_owner is NPCAgent2D, "Only NPC agents can close investigations");
            _worldState.CloseInvestigation(_owner as NPCAgent2D);
        }

        public void SolveInvestigation()
        {
            DebugTool.Assert(_owner is NPCAgent2D, "Only NPC agents can solve investigations");
            _worldState.SolveInvestigation(_owner as NPCAgent2D);
        }
    }
}