using System;
using Godot;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Components.Variants;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.States
{
    /// <summary>
    /// Represents the state where an NPC agent attempts to steal resources.
    /// </summary>
    public class StealState : BaseState, INavigationState
    {
        private readonly ActorTag2D _target;
        private readonly ResourceType _targetResType;

        private bool _isTargetReached = false;
        private float _amountToSteal = 0;

        /// <summary>
        /// Event triggered when the state is completed.
        /// </summary>
        public event Action CompleteState;

        /// <summary>
        /// Initializes a new instance of the <see cref="StealState"/> class.
        /// </summary>
        /// <param name="owner">The NPC agent owning this state.</param>
        /// <param name="target">The target actor to steal from.</param>
        /// <param name="type">The type of resource to steal.</param>
        public StealState(NPCAgent2D owner, ActorTag2D target, ResourceType type)
            : base(owner)
        {
            _target = target;
            _targetResType = type;
        }

        /// <summary>
        /// Called when the state is entered.
        /// </summary>
        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} StealState Enter");
            _owner.EmitSignal(NPCAgent2D.SignalName.ActionStateEntered, Variant.From(ActionState.Steal));
            _owner.NotifManager.NavigationComplete += OnNavigationComplete;
        }

        /// <summary>
        /// Called when the state is exited.
        /// </summary>
        public override void Exit()
        {
            _owner.EmitSignal(NPCAgent2D.SignalName.ActionStateExited, Variant.From(ActionState.Steal));
            _owner.NotifManager.NavigationComplete -= OnNavigationComplete;
        }

        /// <summary>
        /// Gets the target position for navigation.
        /// </summary>
        /// <returns>The global position of the target.</returns>
        public Vector2 GetTargetPosition()
        {
            return _target.Parent.GlobalPosition;
        }

        /// <summary>
        /// Gets the resource type and amount to steal.
        /// </summary>
        /// <returns>A tuple containing the resource type and amount to steal.</returns>
        public Tuple<ResourceType, float> GetResourceToSteal()
        {
            return new(_targetResType, _amountToSteal);
        }

        /// <summary>
        /// Determines whether the agent is currently navigating.
        /// </summary>
        /// <returns>True if the agent is navigating; otherwise, false.</returns>
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

            TheftData theftData = new()
            {
                ResourceType = _targetResType,
                Amount = (int)_amountToSteal
            };

            _owner.EmitSignal(NPCAgent2D.SignalName.TheftCompleted, theftData);
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