using System;
using Godot;
using Godot.Collections;
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
            _owner.Sensor.SetTaskRecord(ActionType.Investigate, ActionState.Research);

            Array<Variant> data = new() { _subject };

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
            Error result = _owner.EmitSignal(
                NPCAgent2D.SignalName.ActionStateEntered,
                Variant.From(ActionStateValue),
                new Array<Variant>()
            );
            DebugTool.Assert(result != Error.Unavailable, "Signal emitted error");
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