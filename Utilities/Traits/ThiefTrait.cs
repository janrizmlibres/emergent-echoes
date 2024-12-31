using EmergentEchoes.addons.NPC2DNode;
using EmergentEchoes.addons.NPCNode;
using EmergentEchoes.Entities.Actors;
using EmergentEchoes.Utilities.World;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EmergentEchoes.Utilities.Traits
{
    public class ThiefTrait : Trait
    {
        private readonly NPC _owner;
        private readonly List<ResourceStat> _resources;
        private readonly List<Actor> _actors;
        private readonly float _weight;

        public ThiefTrait(NPC owner, List<ResourceStat> resources, float weight)
        {
            _owner = owner;
            _resources = resources;
            _weight = weight;
            _actors = _sensor.GetActors();
        }

        public override Tuple<string, float> EvaluateAction()
        {
            List<ResourceStat> tangibleResources;
            HashSet<ResourceStat> evaluatedResources = new();
            ResourceStat selectedResource = null;

            tangibleResources = _resources.Where(resource => resource.IsTangible).ToList();

            while (evaluatedResources.Count != _resources.Count && selectedResource == null)
            {
                selectedResource = tangibleResources
                   .Where(resource => resource.IsImbalanced() && !evaluatedResources.Contains(resource))
                   .First();

                selectedResource ??= tangibleResources
                    .Where(resource => !evaluatedResources.Contains(resource))
                    .First();

                float chosenLikelihood = 0;
                Actor chosenActor = null;

                List<Actor> otherActors = _actors
                    .Where(actor => actor.GetInstanceId() != _owner.GetInstanceId()).ToList();

                foreach (Actor actor in otherActors)
                {
                    if (actor.HasResource(selectedResource))
                    {
                        float likelihood = _owner.Thief * _weight * selectedResource.Value;
                        if (likelihood > chosenLikelihood)
                        {
                            chosenLikelihood = likelihood;
                            chosenActor = actor;
                        }
                    }
                }

                if (chosenActor != null)
                {
                    return new Tuple<string, float>($"Steal {selectedResource.ResourceType} from {chosenActor.Name}", chosenLikelihood);
                }

                evaluatedResources.Add(selectedResource);
                selectedResource = null;
            }

            return new Tuple<string, float>(string.Empty, 0f);
        }

        public override bool ShouldActivate(SocialPractice practice)
        {
            return practice.PracticeType == SocialPractice.Practice.Proactive;
        }

    }
}