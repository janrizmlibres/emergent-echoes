using System;
using NPCProcGen.Core.Actions;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.Traits
{
    public class LawfulTrait : Trait
    {
        private const float InvestigationDuration = 600;

        public Crime AssignedCase { get; private set; }

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

            if (AssignedCase == null)
            {
                StartNewInvestigation();
                return;
            }

            ResumeInvestigation();
        }

        private void StartNewInvestigation()
        {
            AssignedCase = _actorCtx.Sensor.AssignCase();
            if (AssignedCase == null) return;

            _investigationTimer = InvestigationDuration;

            AddAction(ActionType.Assess, ResourceType.Duty);
        }

        private void ResumeInvestigation()
        {
            ActorTag2D target = AssignedCase.GetRandomParticipant();

            if (target != null)
            {
                AddAction(ActionType.Interrogate, ResourceType.Duty, target, AssignedCase);
                return;
            }

            if (AssignedCase.IsDeposed())
            {
                AddAction(ActionType.Pursuit, ResourceType.Duty);
            }
        }

        private void AttemptResolveCase()
        {
            if (AssignedCase.IsUnsolvable())
            {
                ClearCase(CrimeStatus.Unsolved);
                return;
            }

            AddAction(ActionType.Pursuit, ResourceType.Duty);
        }

        public void ClearCase(CrimeStatus status)
        {
            if (status == CrimeStatus.Pending) throw new ArgumentException("Invalid status");

            AssignedCase.Status = status;
            AssignedCase = null;
            _investigationTimer = InvestigationDuration;
        }
    }
}