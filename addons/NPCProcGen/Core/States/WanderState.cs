using System;
using Godot;

namespace NPCProcGen.Core.States
{
    public class WanderState : ActionState, ILinearState
    {
        private readonly static Random _rnd = new();
        private static readonly int _min = 5;
        private static readonly int _max = 10;

        private float _maxDuration = 30;
        private float _wanderInterval = _rnd.Next(_min, _max);
        private float _timer = 0;

        private bool _isWandering = false;

        private readonly ActorTag2D _target = null;
        private Vector2 _targetPosition;

        public event Action StateComplete;

        public WanderState(NPCAgent2D owner, ActorTag2D target) : base(owner)
        {
            _target = target;
        }

        public override void Enter()
        {
            GD.Print("WanderState Enter");
            _owner.NotifManager.NavigationComplete += OnNavigationComplete;
            _owner.NotifManager.ActorDetected += OnActorDetected;
        }

        public override void Update(double delta)
        {
            if (!_isWandering)
            {
                _timer += (float)delta;
            }

            _maxDuration -= (float)delta;

            if (_timer >= _wanderInterval)
            {
                _timer = 0;
                _wanderInterval = _rnd.Next(_min, _max);
                _isWandering = true;
            }

            if (_maxDuration <= 0)
            {
                StateComplete?.Invoke();
            }
        }

        public override Vector2 GetTargetPosition()
        {
            return _target?.Parent.GlobalPosition ?? _targetPosition;
        }

        private void OnNavigationComplete()
        {
            _isWandering = false;
        }

        private void OnActorDetected(ActorTag2D actor)
        {
            if (_target == actor)
            {
                StateComplete?.Invoke();
            }
        }
    }
}