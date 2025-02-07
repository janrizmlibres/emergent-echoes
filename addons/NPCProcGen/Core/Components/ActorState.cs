using System;
using NPCProcGen.Core.Components.Enums;

namespace NPCProcGen.Core.Components
{
    public class ActorState
    {
        public Tuple<ActionType, ActionState> CurrentTask { get; set; }
        public ResourceType? CurrentPetitionResourceType { get; set; }
    }
}