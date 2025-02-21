using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using NPCProcGen.Core.Actions;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.Traits
{
    /// <summary>
    /// Represents a trait for lawful actions.
    /// </summary>
    public class LawfulTrait : Trait
    {
        private const float CrimeInvestigationTime = 600;

        public Crime AssignedCrime
        {
            get => _assignedCrime;
            set
            {
                _assignedCrime = value;
                _investigationTimer = CrimeInvestigationTime;
            }
        }

        private Crime _assignedCrime = null;
        private float _investigationTimer = CrimeInvestigationTime;

        public LawfulTrait(ActorContext context, float weight) : base(context, weight) { }

        public override void Update(double delta)
        {
            if (AssignedCrime == null) return;

            _investigationTimer -= (float)delta;

            if (_investigationTimer <= 0)
            {
                MarkCrimeAsUnsolved();
            }
        }

        /// <summary>
        /// Evaluates an action based on the given social practice.
        /// </summary>
        /// <param name="practice">The social practice to evaluate.</param>
        /// <returns>A tuple containing the evaluated action and its weight.</returns>
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
            List<Tuple<BaseAction, float>> actionCandidates = new();

            if (AssignedCrime == null)
                StartNewInvestigation(actionCandidates);
            else
                ContinueInvestigation(actionCandidates);

            return actionCandidates.OrderByDescending(tuple => tuple.Item2).FirstOrDefault();
        }

        private void StartNewInvestigation(List<Tuple<BaseAction, float>> actionCandidates)
        {
            AssignedCrime = _actorCtx.Sensor.InvestigateCrime();
            if (AssignedCrime == null) return;

            AddSimpleAction(
                actionCandidates,
                () => new InvestigateAction(_actorCtx, AssignedCrime), _weight
            );
        }

        private void ContinueInvestigation(List<Tuple<BaseAction, float>> actionCandidates)
        {
            if (AssignedCrime.HasRemainingWitnesses())
            {
                AddSimpleAction(
                    actionCandidates,
                    () => new InvestigateAction(_actorCtx, AssignedCrime), _weight
                );
                return;
            }

            float successRate = AssignedCrime.GetSolveRate();

            if (GD.Randf() <= successRate)
            {
                AddSimpleAction(
                    actionCandidates,
                    () => new PursuitAction(_actorCtx, AssignedCrime.Criminal), _weight
                );
                return;
            }

            AddSimpleAction(actionCandidates, () => new CloseCaseAction(_actorCtx), _weight);
        }

        public void MarkCrimeAsUnsolved()
        {
            AssignedCrime = null;
            _actorCtx.Sensor.CloseInvestigation();
        }

        public void MarkCrimeAsSolved()
        {
            AssignedCrime = null;
            _actorCtx.Sensor.SolveInvestigation();
        }
    }
}