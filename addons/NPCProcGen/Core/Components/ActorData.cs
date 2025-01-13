using Godot;

namespace NPCProcGen.Core.Components
{
    public class ActorData
    {
        private static readonly float DECAY_DURATION = 60; // 1 minute

        public Vector2? LastKnownPosition { get; set; } = null;
        public float Relationship { get; private set; } = 0;

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

            if (_decayTimer >= DECAY_DURATION)
            {
                LastKnownPosition = null;
                _decayTimer = 0;
            }
        }
    }
}