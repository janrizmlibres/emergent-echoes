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

        /// <summary>
        /// Initializes a new instance of the <see cref="LawfulTrait"/> class.
        /// </summary>
        /// <param name="owner">The owner of the trait.</param>
        /// <param name="weight">The weight of the trait.</param>
        /// <param name="sensor">The sensor associated with the trait.</param>
        /// <param name="memorizer">The memorizer associated with the trait.</param>
        public LawfulTrait(NPCAgent2D owner, float weight, Sensor sensor, NPCMemorizer memorizer)
            : base(owner, weight, sensor, memorizer) { }

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
            AssignedCrime = _sensor.InvestigateCrime();
            if (AssignedCrime == null) return;

            AddSimpleAction(
                actionCandidates,
                () => new InvestigateAction(_owner, AssignedCrime), _weight
            );
        }

        private void ContinueInvestigation(List<Tuple<BaseAction, float>> actionCandidates)
        {
            if (AssignedCrime.HasRemainingWitnesses())
            {
                AddSimpleAction(
                    actionCandidates,
                    () => new InvestigateAction(_owner, AssignedCrime), _weight
                );
                return;
            }

            float successRate = AssignedCrime.GetSolveRate();

            if (GD.Randf() <= successRate)
            {
                AddSimpleAction(
                    actionCandidates,
                    () => new PursuitAction(_owner, AssignedCrime.Criminal), _weight
                );
                return;
            }

            AddSimpleAction(actionCandidates, () => new CloseCaseAction(_owner), _weight);
        }

        public void MarkCrimeAsUnsolved()
        {
            AssignedCrime = null;
            _sensor.CloseInvestigation();
        }

        public void MarkCrimeAsSolved()
        {
            AssignedCrime = null;
            _sensor.SolveInvestigation();
        }
    }
}