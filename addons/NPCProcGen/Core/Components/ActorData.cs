using Godot;

namespace NPCProcGen.Core.Components
{
    public class ActorData
    {
        private static readonly float _decayDuration = 60;

        public Vector2? LastKnownPosition
        {
            get => _lastKnownPosition;
            set
            {
                _lastKnownPosition = value;
                _decayTimer = 0;
            }
        }

        public float Relationship { get; private set; } = 0;

        private Vector2? _lastKnownPosition = null;

        private readonly ActorTag2D _actor;

        private float _decayTimer = 0;

        public ActorData(ActorTag2D actor)
        {
            _actor = actor;
        }

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