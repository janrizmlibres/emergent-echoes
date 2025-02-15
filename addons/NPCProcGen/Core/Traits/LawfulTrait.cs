using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <summary>
        /// Initializes a new instance of the <see cref="LawfulTrait"/> class.
        /// </summary>
        /// <param name="owner">The owner of the trait.</param>
        /// <param name="weight">The weight of the trait.</param>
        /// <param name="sensor">The sensor associated with the trait.</param>
        /// <param name="memorizer">The memorizer associated with the trait.</param>
        public LawfulTrait(NPCAgent2D owner, float weight, Sensor sensor, NPCMemorizer memorizer)
            : base(owner, weight, sensor, memorizer) { }

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

            if (!_memorizer.IsInvestigating())
            {
                Crime crime = _memorizer.StartInvestigation();
                if (crime == null) return null;
                AddSimpleAction(actionCandidates, () => new InvestigateAction(_owner, crime), _weight);
            }

            Crime currentCrime = _memorizer.GetInvestigation();

            Func<BaseAction> actionCreator = currentCrime.HasRemainingWitnesses() ?
                () => new InvestigateAction(_owner, currentCrime) :
                () => new PursuitAction(_owner, currentCrime.Criminal);

            AddSimpleAction(actionCandidates, actionCreator, _weight);
            return actionCandidates.OrderByDescending(tuple => tuple.Item2).FirstOrDefault();
        }
    }
}