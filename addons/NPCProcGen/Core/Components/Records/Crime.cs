using System.Collections.Generic;
using NPCProcGen.Core.Components.Enums;

namespace NPCProcGen.Core.Components.Records
{
    /// <summary>
    /// Represents a crime event.
    /// </summary>
    /// <param name="Category">The category of the crime.</param>
    /// <param name="Criminal">The actor who committed the crime.</param>
    /// <param name="Victim">The actor who was the victim of the crime.</param>
    /// <param name="Witnesses">The list of actors who witnessed the crime.</param>
    public record Crime(CrimeCategory Category, ActorTag2D Criminal, ActorTag2D Victim, IReadOnlyList<ActorTag2D> Witnesses);
}