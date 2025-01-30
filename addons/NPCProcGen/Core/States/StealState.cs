using System;
using Godot;
using Godot.Collections;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.States
{
    /// <summary>
    /// Represents the state where an NPC agent attempts to steal resources.
    /// </summary>
    public class StealState : BaseState
    {
        private readonly ActorTag2D _targetActor;
        private readonly ResourceType _targetResource;
        private readonly float _amountToSteal;

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
            _targetActor = target;
            _targetResource = type;
            _amountToSteal = ComputeStealAmount();
        }

        /// <summary>
        /// Called when the state is entered.
        /// </summary>
        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} StealState Enter");
            _owner.EmitSignal(NPCAgent2D.SignalName.ActionStateEntered, Variant.From(ActionState.Steal));

            ResourceManager.Instance.TranserResources(_targetActor, _owner, _targetResource, _amountToSteal);
            CompleteState?.Invoke();
        }

        /// <summary>
        /// Called when the state is exited.
        /// </summary>
        public override void Exit()
        {
            Array<Variant> theftData = new()
            {
                Variant.From(_targetResource),
                _amountToSteal
            };

            _owner.EmitSignal(NPCAgent2D.SignalName.ActionStateExited, Variant.From(ActionState.Steal), theftData);
        }

        /// <summary>
        /// Computes the amount of resources to steal based on thief's need, resource weight, and thresholds.
        /// </summary>
        /// <returns>The amount of resources to steal.</returns>
        private float ComputeStealAmount()
        {
            ResourceStat ownerResource = ResourceManager.Instance.GetResource(_owner, _targetResource);
            ResourceStat targetResource = ResourceManager.Instance.GetResource(_targetActor, _targetResource);
            return CommonUtils.CalculateSkewedAmount(ownerResource, 0.5f, 2, targetResource.Amount);
        }
    }
}