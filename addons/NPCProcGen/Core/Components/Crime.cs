using System.Collections.Generic;
using System.Linq;
using Godot;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.Components
{
    public enum CrimeStatus
    {
        Pending,
        Unsolved,
        Solved
    }

    public class Crime
    {
        public NPCAgent2D Investigator { get; set; }
        public CrimeStatus Status { get; set; }

        public CrimeCategory Category { get; private set; }
        public ActorTag2D Criminal { get; private set; }
        public List<ActorTag2D> Participants { get; private set; } = new();

        private readonly List<ActorTag2D> _verifiers = new();
        private readonly List<ActorTag2D> _falsifiers = new();

        public Crime(CrimeCategory category, ActorTag2D criminal)
        {
            Category = category;
            Status = CrimeStatus.Pending;
            Criminal = criminal;
        }

        public bool IsOpen()
        {
            return Investigator == null && Status == CrimeStatus.Pending;
        }

        public ActorTag2D GetRandomParticipant()
        {
            return CommonUtils.Shuffle(Participants.Where(actor => actor.IsValidTarget(Investigator))
                .ToList())
                .FirstOrDefault();
        }

        public void ClearParticipant(ActorTag2D actor, bool isSuccess)
        {
            if (isSuccess)
            {
                _verifiers.Add(actor);
            }
            else
            {
                _falsifiers.Add(actor);
            }
        }

        public bool IsDeposed()
        {
            DebugTool.Assert(
                _verifiers.Count + _falsifiers.Count <= Participants.Count,
                "Interrogations exceed participants count."
            );
            return _verifiers.Count + _falsifiers.Count == Participants.Count;
        }

        public bool IsUnsolvable()
        {
            float probability = Participants.Count > 6
                ? _verifiers.Count / (float)Participants.Count
                : 0.1f + 0.15f * _verifiers.Count;

            return GD.Randf() > probability;
        }
    }
}