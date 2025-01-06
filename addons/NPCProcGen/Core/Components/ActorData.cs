using Godot;

namespace NPCProcGen.Core.Components
{
    public class ActorData
    {
        private static readonly float DECAY_DURATION = 60 * 5; // 5 minutes

        public Vector2? LastKnownPosition { get; private set; }
        public float Relationship { get; private set; }

        private readonly ActorTag2D _actor = null;

        private float _decayTimer = 0;

        public ActorData(ActorTag2D actor)
        {
            _actor = actor;
            LastKnownPosition = null;
            Relationship = 0;
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