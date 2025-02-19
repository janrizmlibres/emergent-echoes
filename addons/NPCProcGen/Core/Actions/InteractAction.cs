using Godot;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.States;

namespace NPCProcGen.Core.Actions
{
    public class InteractAction : BaseAction, IInteractionAction
    {
        public const ActionType ActionTypeValue = ActionType.Interact;

        private readonly ActorTag2D _target;
        private readonly InteractionState _interactState;

        public InteractAction(NPCAgent2D owner, ActorTag2D target) : base(owner)
        {
            _target = target;
            _interactState = new(owner, ActionTypeValue, target);
        }

        public void Subscribe()
        {
            _target.NotifManager.ActorImprisoned += InterruptAction;
        }

        public void Unsubscribe()
        {
            _target.NotifManager.ActorImprisoned -= InterruptAction;
        }

        public override void Update(double delta)
        {
            _currentState?.Update(delta);
        }

        public override void Run()
        {
            Error result = _owner.EmitSignal(
                NPCAgent2D.SignalName.ExecutionStarted,
                Variant.From(ActionTypeValue)
            );
            DebugTool.Assert(result != Error.Unavailable, "Signal emitted error");

            TransitionTo(_interactState);
        }
    }
}