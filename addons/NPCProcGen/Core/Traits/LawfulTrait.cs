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

        public override Tuple<BaseAction, float> EvaluateAction(SocialPractice practice)
        {
            if (practice == SocialPractice.Proactive)
            {
                return EvaluateProactiveAction();
            }

            return null;
        }

        private Tuple<BaseAction, float> EvaluateProactiveAction()
        {
            if (_investigationTimer <= 0) return AttemptResolveCase();

            if (AssignedCase == null)
            {
                return StartNewInvestigation();
            }

            return ResumeInvestigation();
        }

        private Tuple<BaseAction, float> StartNewInvestigation()
        {
            AssignedCase = _actorCtx.Sensor.AssignCase();
            if (AssignedCase == null) return null;

            _investigationTimer = InvestigationDuration;

            BaseAction action = new AssessAction(_actorCtx);
            return new(action, _weight);
        }

        private Tuple<BaseAction, float> ResumeInvestigation()
        {
            ActorTag2D target = AssignedCase.GetRandomParticipant();

            if (target != null)
            {
                BaseAction interrogateAction = new InterrogateAction(_actorCtx, target, AssignedCase);
                return new(interrogateAction, _weight);
            }

            if (AssignedCase.IsDeposed())
            {
                BaseAction pursuitAction = new PursuitAction(_actorCtx);
                return new(pursuitAction, _weight);
            }

            return null;
        }

        private Tuple<BaseAction, float> AttemptResolveCase()
        {
            if (AssignedCase.IsUnsolvable())
            {
                ClearCase(CrimeStatus.Unsolved);
                return null;
            }

            BaseAction pursuitAction = new PursuitAction(_actorCtx);
            return new(pursuitAction, _weight);
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