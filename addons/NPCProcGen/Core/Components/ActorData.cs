using System;
using Godot;

namespace NPCProcGen.Core.Components
{
    /// <summary>
    /// Represents data related to an actor.
    /// </summary>
    public class ActorData
    {
        private const float DecayDuration = 60;

        /// <summary>
        /// Gets or sets the last known position of the actor.
        /// </summary>
        public Vector2? LastKnownPosition
        {
            get => _lastKnownPosition;
            set
            {
                _lastKnownPosition = value;
                _decayTimer = 0;
            }
        }

        /// <summary>
        /// Gets the relationship value of the actor.
        /// </summary>
        public float Relationship
        {
            get => _relationship;
            set => _relationship = Math.Clamp(value, -35, 35);
        }

        private Vector2? _lastKnownPosition = null;
        private float _relationship = 0;
        private float _decayTimer = 0;

        private readonly ActorTag2D _actor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorData"/> class.
        /// </summary>
        /// <param name="actor">The actor associated with this data.</param>
        public ActorData(ActorTag2D actor)
        {
            _actor = actor;
        }

        /// <summary>
        /// Updates the actor data.
        /// </summary>
        /// <param name="delta">The time elapsed since the last update.</param>
        public void Update(double delta)
        {
            if (LastKnownPosition == null) return;

            _decayTimer += (float)delta;

            if (_decayTimer >= DecayDuration)
            {
                GD.Print($"{_actor.Parent.Name}'s last known position has decayed.");
                LastKnownPosition = null;
            }
        }
    }
}