using System;
using Godot;
using Godot.Collections;
using NPCProcGen.Core.Actions;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.States
{
    public class InterrogateState : BaseState
    {
        public const ActionState ActionStateValue = ActionState.Interrogate;

        private const int MinDuration = 10;
        private const int MaxDuration = 20;

        private readonly Crime _crime;
        private readonly ActorTag2D _subject;
        private float _duration;

        public event Action CompleteState;

        public InterrogateState(NPCAgent2D owner, ActionType action, Crime crime, ActorTag2D subject)
            : base(owner, action)
        {
            _crime = crime;
            _subject = subject;
            _duration = GD.RandRange(MinDuration, MaxDuration);
        }

        public override void Enter()
        {
            GD.Print($"{_owner.Parent.Name} InterrogateState Enter");

            if (_subject is NPCAgent2D npc)
            {
                npc.AddAction(new InteractAction(npc, _owner));
            }
            else
            {
                _subject.NotifManager.NotifyInteractionStarted();
                _subject.Sensor.SetTaskRecord(ActionType.Interact, ActionState.Interact);

                Array<Variant> targetData = new() { _owner.Parent, };

                Error targetResult = _subject.EmitSignal(
                    ActorTag2D.SignalName.InteractionStarted,
                    Variant.From((InteractState)ActionStateValue),
                    targetData
                );
                DebugTool.Assert(targetResult != Error.Unavailable, "Signal emitted error");
            }

            _owner.NotifManager.NotifyInteractionStarted();
            _owner.Sensor.SetTaskRecord(_actionType, ActionStateValue);

            Array<Variant> data = new() { _subject.Parent };

            Error result = _owner.EmitSignal(
                NPCAgent2D.SignalName.ActionStateEntered,
                Variant.From(ActionStateValue),
                data
            );
            DebugTool.Assert(result != Error.Unavailable, "Signal emitted error");
        }

        public override void Update(double delta)
        {
            _duration -= (float)delta;

            if (_duration <= 0)
            {
                CalculateSuccess();
                CompleteState?.Invoke();
            }
        }

        public override void Exit()
        {
            _owner.NotifManager.NotifyInteractionEnded();
            _owner.Sensor.ClearTaskRecord();

            Array<Variant> data = new() { _subject.Parent };

            Error result = _owner.EmitSignal(
                NPCAgent2D.SignalName.ActionStateEntered,
                Variant.From(ActionStateValue),
                data
            );
            DebugTool.Assert(result != Error.Unavailable, "Signal emitted error");

            if (_subject is NPCAgent2D npc)
            {
                npc.EndAction();
            }
            else
            {
                _subject.NotifManager.NotifyInteractionEnded();
                _subject.Sensor.ClearTaskRecord();

                result = _subject.EmitSignal(ActorTag2D.SignalName.InteractionEnded);
                DebugTool.Assert(result != Error.Unavailable, "Signal emitted error");
            }
        }

        private void CalculateSuccess()
        {
            float relationship = _owner.Memorizer.GetActorRelationship(_subject);
            float successRate = ActorData.GetInterrogationProbability(relationship);

            if (GD.Randf() <= successRate)
            {
                _crime.MarkSuccessfulWitness(_subject);
            }
            else
            {
                _crime.MarkFailedWitness(_subject);
            }
        }
    }
}