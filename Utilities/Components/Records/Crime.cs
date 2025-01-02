using System;
using System.Collections.Generic;
using EmergentEchoes.addons.NPC2DNode;
using EmergentEchoes.Utilities.Components.Enums;

namespace EmergentEchoes.Utilities.Components.Records
{
    public record Crime(CrimeType Type, NPC2D Criminal, NPC2D Victim, IReadOnlyList<NPC2D> Witnesses);
}