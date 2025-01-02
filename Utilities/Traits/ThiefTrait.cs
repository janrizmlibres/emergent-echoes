using EmergentEchoes.addons.NPC2DNode;
using EmergentEchoes.Entities.Actors;
using EmergentEchoes.Utilities.Actions;
using EmergentEchoes.Utilities.Components;
using EmergentEchoes.Utilities.Components.Enums;
using EmergentEchoes.Utilities.Internal;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EmergentEchoes.Utilities.Traits
{
    public class ThiefTrait : Trait
    {
        private readonly List<Actor> _actors;

        public ThiefTrait(NPC2D owner, Memorizer memorizer, float weight) : base(owner, memorizer, weight)
        {
            _actors = _sensor.GetActors();
        }

        public override Tuple<NPCAction, float> EvaluateAction()
        {
            List<ResourceStat> tangibleResources;
            HashSet<ResourceStat> evaluatedResources = new();
            ResourceStat selectedResource = null;

            tangibleResources = _owner.Resources.Where(resource => resource.IsTangible).ToList();

            while (evaluatedResources.Count != _owner.Resources.Count && selectedResource == null)
            {
                selectedResource = tangibleResources
                   .Where(resource => resource.IsDeficient() && !evaluatedResources.Contains(resource))
                   .First();

                selectedResource ??= tangibleResources
                    .Where(resource => !evaluatedResources.Contains(resource))
                    .First();

                Actor chosenActor = null;

                List<Actor> otherActors = _actors
                    .Where(actor => actor != _owner)
                    .ToList();

                foreach (Actor actor in otherActors)
                {
                    // TODO: Check also if imbalance is not too severe
                    // TODO: Evaluate probability or likelihood of success
                    if (actor.HasResource(selectedResource) && !_memorizer.IsLiked(actor))
                    {
                        chosenActor = actor;
                    }
                }

                if (chosenActor != null)
                {
                    float imbalance = selectedResource.LowerThreshold - selectedResource.Value;
                    float unweightedScore = Math.Max(0, imbalance) / selectedResource.LowerThreshold;
                    float weightedScore = unweightedScore * selectedResource.Weight * _weight;

                    TheftAction action = new(_owner, chosenActor);
                    return new Tuple<NPCAction, float>(action, weightedScore);
                }

                evaluatedResources.Add(selectedResource);
                selectedResource = null;
            }

            return new Tuple<NPCAction, float>(null, 0);
        }

        public override bool ShouldActivate(SocialPractice practice)
        {
            return practice == SocialPractice.Proactive;
        }
    }
}