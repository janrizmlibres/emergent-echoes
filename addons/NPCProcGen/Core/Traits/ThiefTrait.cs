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
    public class ThiefTrait : Trait
    {
        private static readonly Random _rng = new();

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
                selectedType = UnevaluatedTypes.FirstOrDefault(resource => _owner.Resources[resource].IsDeficient());
                selectedType ??= UnevaluatedTypes.First();

                ActorTag2D chosenActor = null;

                List<ActorTag2D> otherActors = _actors
                    .Where(actor => actor != _owner)
                    .OrderBy(_ => _rng.Next())
                    .ToList();

                Vector2? actorLastPos = null;

                foreach (ActorTag2D actor in otherActors)
                {
                    actorLastPos = _owner.Memorizer.GetActorLocation(actor);

                    // TODO: Add check if actor workplace is known
                    // TODO: Check also if imbalance is not too severe
                    // TODO: Evaluate probability or likelihood of success if applicable
                    if (actorLastPos.HasValue
                        && actor.HasResource((ResourceType)selectedType)
                        && !_memorizer.IsTrusted(actor))
                    {
                        chosenActor = actor;
                        break;
                    }
                }

                if (chosenActor != null)
                {
                    ResourceStat chosenResource = _owner.Resources[selectedType.Value];

                    float imbalance = chosenResource.LowerThreshold - chosenResource.Value;
                    float unweightedScore = Math.Max(0, imbalance) / chosenResource.LowerThreshold;
                    float weightedScore = unweightedScore * chosenResource.Weight * _weight;

                    TheftAction action = new(_owner, chosenActor, selectedType.Value);
                    return new Tuple<NPCAction, float>(action, weightedScore);
                }

                UnevaluatedTypes.Remove(selectedType.Value);
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