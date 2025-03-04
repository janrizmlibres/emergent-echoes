using NPCProcGen.Core.Actions;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.Traits
{
    public class LawfulTrait : Trait
    {
        private const float InvestigationDuration = 600;

        private Crime _assignedCase;
        public Crime AssignedCase
        {
            get => _assignedCase;
            private set
            {
                _assignedCase = value;
                _investigationTimer = InvestigationDuration;

                if (_assignedCase != null)
                {
                    _assignedCase.Investigator = _actorCtx.GetNPCAgent2D();
                    _assignedCase.OnCrimeClosed += () => _assignedCase = null;
                }
            }
        }

        private float _investigationTimer = InvestigationDuration;

        public LawfulTrait(ActorContext context, float weight) : base(context, weight) { }

        public override void Update(double delta)
        {
            if (AssignedCase == null) return;
            _investigationTimer -= (float)delta;
        }

        protected override void EvaluateProactiveAction()
        {
            if (_investigationTimer <= 0)
            {
                AttemptResolveCase();
                return;
            }

            AssignedCase ??= Sensor.GetOpenCase();
            if (AssignedCase == null) return;

            if (!AssignedCase.AssessmentDone)
            {
                AddAction(ActionType.Assess, ResourceType.Duty);
                return;
            }

            if (AssignedCase.IsWitnessed())
            {
                AddAction(ActionType.Pursuit, ResourceType.Duty);
                return;
            }

            ResumeInvestigation();
        }

        public void PursueCriminal(ActorTag2D criminal, Crime crime)
        {
            PursuitAction action = new(_actorCtx, criminal, crime);
            _actorCtx.Executor.AddAction(action);
        }

        private void ResumeInvestigation()
        {
            ActorTag2D target = AssignedCase.GetRandomParticipant();

            if (target != null)
            {
                ActionEvalParams actionParams = new()
                {
                    TargetActor = target,
                    Crime = AssignedCase,
                };

                AddAction(ActionType.Interrogate, ResourceType.Duty, actionParams);
                return;
            }

            if (AssignedCase.IsDeposed())
            {
                AttemptResolveCase();
            }
        }

        private void AttemptResolveCase()
        {
            if (!AssignedCase.IsSolvable())
            {
                ActionEvalParams actionParams = new() { CaseClosed = true };
                AddAction(ActionType.Assess, ResourceType.Duty, actionParams);
                return;
            }

            AddAction(ActionType.Pursuit, ResourceType.Duty);
        }
    }
}