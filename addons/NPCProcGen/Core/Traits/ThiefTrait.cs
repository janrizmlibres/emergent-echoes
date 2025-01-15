using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Actions;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Core.Traits
{
    public class ThiefTrait : Trait
    {
        public ThiefTrait(NPCAgent2D owner, float weight, Sensor sensor, Memorizer memorizer)
            : base(owner, weight, sensor, memorizer) { }

        public override Tuple<NPCAction, float> EvaluateAction(SocialPractice practice)
        {
            if (practice == SocialPractice.Proactive)
            {
                return EvaluateProactiveAction();
            }

            return null;
        }

        private Tuple<NPCAction, float> EvaluateProactiveAction()
        {
            List<ResourceType> unevaluatedTypes = ResourceManager.Instance.TangibleTypes;
            ResourceType? selectedType = null;

            while (unevaluatedTypes.Count > 0 && selectedType == null)
            {
                GD.Print("Evaluating theft action...");
                selectedType = SelectResourceType(unevaluatedTypes);
                DebugTool.Assert(selectedType != null, "Resource type must not be null");

                ActorTag2D chosenActor = ChooseActor(selectedType.Value);

                if (chosenActor != null)
                {
                    return CreateTheftAction(chosenActor, selectedType.Value);
                }

                bool result = unevaluatedTypes.Remove(selectedType.Value);
                DebugTool.Assert(result, "Resource type must be removed from unevaluated types");
                selectedType = null;
            }

            return null;
        }

        private ResourceType SelectResourceType(List<ResourceType> unevaluatedTypes)
        {
            ResourceType? type = null;

            foreach (var resource in unevaluatedTypes)
            {
                if (ResourceManager.Instance.IsDeficient(_owner, resource))
                {
                    type = resource;
                    break;
                }
            }

            // TODO: If random type is chosen, the final weight of the action should be reduced
            return type ?? unevaluatedTypes[CommonUtils.Rnd.Next(unevaluatedTypes.Count)];
        }

        private ActorTag2D ChooseActor(ResourceType type)
        {
            ActorTag2D result = null;

            List<ActorTag2D> otherActors = _sensor.GetActors()
                .Where(actor => actor != _owner)
                .OrderBy(_ => CommonUtils.Rnd.Next())
                .ToList();

            Vector2? actorLastPos = null;

            foreach (ActorTag2D actor in otherActors)
            {
                actorLastPos = _owner.Memorizer.GetActorLocation(actor);

                // TODO: Add check if actor workplace is known
                // TODO: Check also if imbalance is not too severe
                if (actorLastPos != null
                    && ResourceManager.Instance.HasResource(actor, type)
                    && !_memorizer.IsTrusted(actor))
                {
                    result = actor;
                    break;
                }
            }

            return result;
        }

        private Tuple<NPCAction, float> CreateTheftAction(ActorTag2D chosenActor, ResourceType selectedType)
        {
            ResourceStat chosenResource = ResourceManager.Instance.GetResource(_owner, selectedType);

            float imbalance = chosenResource.LowerThreshold - chosenResource.Amount;
            float unweightedScore = Math.Max(0, imbalance) / chosenResource.LowerThreshold;
            float weightedScore = unweightedScore * chosenResource.Weight * _weight;

            TheftAction action = new(_owner, chosenActor, selectedType);
            return new Tuple<NPCAction, float>(action, weightedScore);
        }
    }
}