using System;
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
        public event Action OnCrimeClosed;

        public NPCAgent2D Investigator { get; set; }
        public bool AssessmentDone { get; set; } = false;

        private CrimeStatus _status;
        public CrimeStatus Status
        {
            get => _status;
            set
            {
                _status = value;

                if (value == CrimeStatus.Solved || value == CrimeStatus.Unsolved)
                {
                    OnCrimeClosed?.Invoke();
                    OnCrimeClosed = null;
                }
            }
        }

        public CrimeCategory Category { get; private set; }
        public ActorTag2D Criminal { get; private set; }
        public List<ActorTag2D> Participants { get; private set; } = new();

        private readonly HashSet<ActorTag2D> _verifiers = new();
        private readonly HashSet<ActorTag2D> _falsifiers = new();

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

        public bool IsWitnessed()
        {
            return Participants.Contains(Investigator);
        }

        public ActorTag2D GetRandomParticipant()
        {
            List<ActorTag2D> filteredParticipants = Participants
                .Where(actor =>
                {
                    return actor.IsValidTarget(Investigator) && !_verifiers.Contains(actor)
                        && !_falsifiers.Contains(actor);
                })
                .ToList();

            return CommonUtils.Shuffle(filteredParticipants)
                .ToList()
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
                _verifiers.Count + _falsifiers.Count > Participants.Count,
                "Interrogations exceed participants count."
            );
            return _verifiers.Count + _falsifiers.Count == Participants.Count;
        }

        public bool IsSolvable()
        {
            float probability = Participants.Count > 6
                ? _verifiers.Count / (float)Participants.Count
                : 0.1f + 0.15f * _verifiers.Count;

            return GD.Randf() <= probability;
        }
    }
}