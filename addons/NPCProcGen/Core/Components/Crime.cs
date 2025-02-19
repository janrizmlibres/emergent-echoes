using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.Components
{
    public class Crime
    {
        public NPCAgent2D Investigator
        {
            get => _investigator;
            set
            {
                _investigator = value;
                if (_witnesses.Contains(value))
                {
                    MarkSuccessfulWitness(value);
                }
            }
        }

        public CrimeCategory Category { get; private set; }
        public ActorTag2D Criminal { get; private set; }
        public ActorTag2D Victim { get; private set; }

        private NPCAgent2D _investigator = null;
        private readonly List<ActorTag2D> _witnesses;
        private readonly List<ActorTag2D> _successfulWitnesses = new();
        private readonly List<ActorTag2D> _failedWitnesses = new();

        public Crime(CrimeCategory category, ActorTag2D criminal, ActorTag2D victim,
            List<ActorTag2D> witnesses)
        {
            Category = category;
            Criminal = criminal;
            Victim = victim;
            _witnesses = witnesses;
        }

        public Tuple<ActorTag2D, Vector2> GetRandomWitnessData(NPCAgent2D investigator)
        {
            List<ActorTag2D> shuffledActors = CommonUtils.Shuffle(_witnesses)
                .Where(actor => investigator.Memorizer.GetLastKnownPosition(actor) != null
                || investigator.IsActorInRange(actor))
                .ToList();

            ActorTag2D chosenWitness = shuffledActors
                .Where(actor => !actor.IsImprisoned())
                .FirstOrDefault();

            if (chosenWitness != null)
            {
                return new(chosenWitness, investigator.Memorizer.GetLastKnownPosition(chosenWitness).Value);
            }

            return null;
        }

        public bool HasRemainingWitnesses()
        {
            return _successfulWitnesses.Count + _failedWitnesses.Count < _witnesses.Count;
        }

        public void MarkSuccessfulWitness(ActorTag2D actor)
        {
            _witnesses.Remove(actor);
            _successfulWitnesses.Add(actor);
        }

        public void MarkFailedWitness(ActorTag2D actor)
        {
            _witnesses.Remove(actor);
            _failedWitnesses.Add(actor);
        }

        public float GetSolveRate()
        {
            return _witnesses.Count > 6
                ? _successfulWitnesses.Count / _witnesses.Count
                : 0.1f + 0.15f * _successfulWitnesses.Count;
        }
    }
}