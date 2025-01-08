using System;
using System.Collections.Generic;
using System.Linq;
using NPCProcGen.Core.Actions;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.Traits
{
    public class ThiefTrait : Trait
    {
        private readonly IReadOnlyList<ActorTag2D> _actors;

        public ThiefTrait(NPCAgent2D owner, float weight, Sensor sensor, Memorizer memorizer)
            : base(owner, weight, sensor, memorizer)
        {
            _actors = sensor.GetActors();
        }

        public override Tuple<NPCAction, float> EvaluateAction()
        {
            List<ResourceType> UnevaluatedTypes = ResourceStat.TangibleTypes.ToList();
            ResourceType? selectedType = null;

            while (UnevaluatedTypes.Count > 0 && selectedType == null)
            {
                selectedType = UnevaluatedTypes.First(resource => _owner.Resources[resource].IsDeficient());
                selectedType ??= UnevaluatedTypes.First();

                ActorTag2D chosenActor = null;

                List<ActorTag2D> otherActors = _actors
                    .Where(actor => actor != _owner)
                    .ToList();

                foreach (ActorTag2D actor in otherActors)
                {
                    // TODO: Check also if imbalance is not too severe
                    // TODO: Evaluate probability or likelihood of success
                    if (actor.HasResource((ResourceType)selectedType) && !_memorizer.IsTrusted(actor))
                    {
                        chosenActor = actor;
                    }
                }

                if (chosenActor != null)
                {
                    ResourceStat chosenResource = _owner.Resources[(ResourceType)selectedType];

                    float imbalance = chosenResource.LowerThreshold - chosenResource.Value;
                    float unweightedScore = Math.Max(0, imbalance) / chosenResource.LowerThreshold;
                    float weightedScore = unweightedScore * chosenResource.Weight * _weight;

                    TheftAction action = new(_owner, chosenActor);
                    return new Tuple<NPCAction, float>(action, weightedScore);
                }

                UnevaluatedTypes.Remove((ResourceType)selectedType);
                selectedType = null;
            }

            return new(null, 0);
        }

        public override bool ShouldActivate(SocialPractice practice)
        {
            return practice == SocialPractice.Proactive;
        }

    }
}