using Godot;

namespace NPCProcGen.Core.Components
{
    /// <summary>
    /// Represents data related to an actor.
    /// </summary>
    public class ActorData
    {
        private static readonly float _decayDuration = 60;

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
        public float Relationship { get; private set; } = 0;

        private Vector2? _lastKnownPosition = null;

        private readonly ActorTag2D _actor;

        private float _decayTimer = 0;

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

            if (_decayTimer >= _decayDuration)
            {
                LastKnownPosition = null;
            }
        }
    }
}