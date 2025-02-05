using System;
using Godot;

namespace NPCProcGen.Core.Components
{
    public class ActorData
    {
        public float Relationship
        {
            get => _relationship;
            set => _relationship = Math.Clamp(value, -35, 35);
        }

        protected readonly ActorTag2D _actor;
        private float _relationship = 0;

        public ActorData(ActorTag2D actor)
        {
            _actor = actor;
        }

        public static float GetBasePetitionProbability(float relationshipLevel)
        {
            return relationshipLevel switch
            {
                <= -26 => 0.05f,
                <= -16 => 0.20f,
                <= -6 => 0.30f,
                <= 4 => 0.40f,
                <= 14 => 0.60f,
                <= 24 => 0.80f,
                _ => 0.95f,
            };
        }

        public bool IsFriendly() => _relationship >= 5;
        public bool IsTrusted() => _relationship >= 15;
        public bool IsClose() => _relationship >= 25;

        public virtual void Update(double delta) { }
    }

    // Hostile: -35 to -26
    // Distrusted: -25 to -16
    // Unfriendly: -15 to -6
    // Neutral: -5 to 4
    // Friendly: 5 to 14
    // Trusted: 15 to 24
    // Close: 25 to 35

    /// <summary>
    /// Represents data related to an actor.
    /// </summary>
    public class NPCActorData : ActorData
    {
        private const float DecayDuration = 60;

        public Vector2? LastKnownPosition
        {
            get => _lastKnownPosition;
            set
            {
                _lastKnownPosition = value;
                _decayTimer = DecayDuration;
            }
        }

        private Vector2? _lastKnownPosition = null;
        private float _decayTimer = DecayDuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorData"/> class.
        /// </summary>
        /// <param name="actor">The actor associated with this data.</param>
        public NPCActorData(ActorTag2D actor) : base(actor) { }

        /// <summary>
        /// Updates the actor data.
        /// </summary>
        /// <param name="delta">The time elapsed since the last update.</param>
        public override void Update(double delta)
        {
            if (_lastKnownPosition == null) return;

            _decayTimer -= (float)delta;

            if (_decayTimer <= 0)
            {
                GD.Print($"{_actor.Parent.Name}'s last known position has decayed.");
                _lastKnownPosition = null;
            }
        }
    }
}