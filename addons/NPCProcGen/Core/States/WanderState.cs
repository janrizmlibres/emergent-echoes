using System;
using Godot;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.States
{
    public class WanderState : ActionState, INavigationState
    {
        private static readonly float _wanderRadius = 100;
        private static readonly int _min = 5;
        private static readonly int _max = 10;

        private readonly ActorTag2D _target;
        private Vector2 _targetPosition;

        // TODO: Should be higher in final implementation
        private float _maxDuration = 30;
        private float _wanderInterval;
        private float _timer = 0;
        private bool _isWandering = false;

        public event Action CompleteState;

        public WanderState(NPCAgent2D owner, ActorTag2D target) : base(owner)
        {
            _target = target;
            _targetPosition = owner.Parent.GlobalPosition;
            _wanderInterval = CommonUtils.Rnd.Next(_min, _max);
        }

        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} WanderState Enter");
            _owner.NotifManager.NavigationComplete += OnNavigationComplete;
        }

        public override void Update(double delta)
        {
            if (_owner.IsActorInRange(_target))
            {
                OnCompleteState();
            }

            if (!_isWandering)
            {
                _timer += (float)delta;

                if (_timer >= _wanderInterval)
                {
                    _timer = 0;
                    _wanderInterval = CommonUtils.Rnd.Next(_min, _max);
                    _targetPosition = CommonUtils.GetRandomPosInCircularArea(
                        _owner.Parent.GlobalPosition, _wanderRadius
                    );
                    _isWandering = true;
                }
            }

            _maxDuration -= (float)delta;

            if (_maxDuration <= 0)
            {
                OnCompleteState();
            }
        }

        public bool IsNavigating()
        {
            return _isWandering;
        }

        public Vector2 GetTargetPosition()
        {
            return _targetPosition;
        }

        private void OnNavigationComplete()
        {
            _isWandering = false;
        }

        private void OnCompleteState()
        {
            _owner.NotifManager.NavigationComplete -= OnNavigationComplete;
            CompleteState?.Invoke();
        }
    }
}