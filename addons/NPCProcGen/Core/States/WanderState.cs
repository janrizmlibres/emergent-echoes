using System;
using Godot;

namespace NPCProcGen.Core.States
{
    public class WanderState : ActionState, ILinearState
    {
        private readonly static Random _rnd = new();

        private static readonly int _min = 5;
        private static readonly int _max = 10;

        private float _totalDuration = 30;
        private float _wanderInterval = _rnd.Next(_min, _max);
        private float _timer = 0;

        private Vector2 _target;
        private bool _isWandering = false;

        public event Action OnComplete;

        public WanderState(NPCAgent2D owner) : base(owner)
        {
        }

        public override void Update(double delta)
        {
            if (!_isWandering)
            {
                _timer += (float)delta;
            }

            _totalDuration -= (float)delta;

            if (_timer >= _wanderInterval)
            {
                _timer = 0;
                _wanderInterval = _rnd.Next(_min, _max);
                _isWandering = true;
            }

            if (_totalDuration <= 0)
            {
                OnComplete?.Invoke();
            }
        }

        public override void CompleteNavigation()
        {
            _isWandering = false;
        }

        public override void CompleteState()
        {
            OnComplete?.Invoke();
        }

        public override Vector2 GetTargetPosition()
        {
            return _target;
        }
    }
}