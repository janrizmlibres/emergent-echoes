using System.Collections.Generic;
using NPCProcGen.Core.Components.Enums;

namespace NPCProcGen.Core.Components.Records
{
    public record Crime(CrimeCategory Category, ActorTag2D Criminal, ActorTag2D Victim, IReadOnlyList<ActorTag2D> Witnesses);
}