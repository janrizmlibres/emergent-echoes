using System;
using Godot;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.States
{
    public class StealState : BaseState, INavigationState
    {
        private readonly ActorTag2D _target;
        private readonly ResourceType _targetResType;

        private bool _isTargetReached = false;
        private float _amountToSteal = 0;

        public event Action CompleteState;

        public StealState(NPCAgent2D owner, ActorTag2D target, ResourceType type)
            : base(owner)
        {
            _target = target;
            _targetResType = type;
        }

        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} StealState Enter");
            _owner.EmitSignal(NPCAgent2D.SignalName.ActionStateEntered, Variant.From(ActionState.Steal));
            _owner.NotifManager.NavigationComplete += OnNavigationComplete;
        }

        public override void Exit()
        {
            _owner.EmitSignal(NPCAgent2D.SignalName.ActionStateExited, Variant.From(ActionState.Steal));
            _owner.NotifManager.NavigationComplete -= OnNavigationComplete;
        }

        public Vector2 GetTargetPosition()
        {
            return _target.Parent.GlobalPosition;
        }

        public Tuple<ResourceType, float> GetResourceToSteal()
        {
            return new(_targetResType, _amountToSteal);
        }

        public bool IsNavigating()
        {
            return !_isTargetReached;
        }

        private void OnNavigationComplete()
        {
            _isTargetReached = true;
            _amountToSteal = ComputeStealAmount();
            ResourceManager.Instance.TranserResources(_target, _owner, _targetResType, _amountToSteal);
            CompleteState?.Invoke();
        }

        private float ComputeStealAmount()
        {
            ResourceStat thiefResource = ResourceManager.Instance.GetResource(_owner, _targetResType);

            float thiefNeed = 1 - thiefResource.Amount / thiefResource.LowerThreshold;
            thiefNeed = Math.Max(0, thiefNeed);

            DebugTool.Assert(thiefNeed >= 0 && thiefNeed <= 1, @"Thief need must be greater than 0 
                and less than or equal to 1");

            float maxStealAmount = thiefResource.UpperThreshold - thiefResource.Amount - 1;
            float minStealAmount = 15;

            double rndValue = CommonUtils.Rnd.NextDouble();
            double exponent = 2 + 3 * (1 - thiefNeed * thiefResource.Weight);
            float skewedvalue = (float)Math.Pow(rndValue, exponent);

            return minStealAmount + (float)skewedvalue * (maxStealAmount - minStealAmount);
        }
    }
}