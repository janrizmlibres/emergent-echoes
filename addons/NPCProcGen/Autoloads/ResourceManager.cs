using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Autoloads
{
    public sealed class ResourceManager
    {
        private static readonly Lazy<ResourceManager> _instance = new(() => new ResourceManager());

        public static ResourceManager Instance => _instance.Value;

        private ResourceManager() { }

        public List<ResourceType> TangibleTypes => _tangibleTypes.ToList();

        private readonly ResourceType[] _tangibleTypes = new[]
        {
            ResourceType.Money,
            ResourceType.Food
        };

        public static List<ResourceType> GetResourceTypes()
        {
            return Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>().ToList();
        }

        public static bool IsTangible(ResourceType type)
        {
            return type == ResourceType.Money || type == ResourceType.Food;
        }

        /// <summary>
        /// Dictionary to store resources for each actor.
        /// </summary>
        private readonly Dictionary<ActorTag2D, Dictionary<ResourceType, ResourceStat>> _actorResources = new();

        /// <summary>
        /// Initializes the resource manager with a list of actors.
        /// </summary>
        /// <param name="actors">The list of actors to initialize.</param>
        public void Initialize(List<ActorTag2D> actors)
        {
            foreach (ActorTag2D actor in actors)
            {
                Dictionary<ResourceType, ResourceStat> resources = new();

                if (actor is NPCAgent2D npc)
                {
                    resources[ResourceType.Money] = new ResourceStat(ResourceType.Money,
                        npc.MoneyAmount, npc.Money);
                    resources[ResourceType.Food] = new ResourceStat(ResourceType.Food,
                        npc.FoodAmount, npc.Food);
                    resources[ResourceType.Satiation] = new ResourceStat(ResourceType.Satiation,
                        npc.SatiationAmount, npc.Satiation);
                    resources[ResourceType.Companionship] = new ResourceStat(ResourceType.Companionship,
                        npc.CompanionshipAmount, npc.Companionship);
                }
                else
                {
                    resources[ResourceType.Money] = new ResourceStat(
                        ResourceType.Money, actor.MoneyAmount, 1);
                    resources[ResourceType.Food] = new ResourceStat(
                        ResourceType.Food, actor.FoodAmount, 1);
                }

                _actorResources[actor] = resources;
            }
        }

        public void Update(double delta)
        {
            foreach (ActorTag2D actor in _actorResources.Keys)
            {
                foreach (ResourceType type in _actorResources[actor].Keys)
                {
                    ResourceStat resource = _actorResources[actor][type];
                    resource.Update(delta);
                }
            }
        }

        public ResourceStat GetResource(ActorTag2D actor, ResourceType type)
        {
            DebugTool.Assert(_actorResources.ContainsKey(actor),
                $"Actor {actor.GetParent<Node2D>().Name} not found in resource manager.");
            return _actorResources[actor].GetValueOrDefault(type);
        }

        public float GetResourceAmount(ActorTag2D actor, ResourceType type)
        {
            DebugTool.Assert(_actorResources.ContainsKey(actor),
                $"Actor {actor.GetParent<Node2D>().Name} not found in resource manager.");
            return _actorResources[actor].GetValueOrDefault(type)?.Amount ?? 0;
        }

        /// <summary>
        /// Checks if a given actor has a specific resource.
        /// </summary>
        /// <param name="actor">The actor to check.</param>
        /// <param name="type">The type of resource to check for.</param>
        /// <returns>True if the actor has the resource, otherwise false.</returns>
        public bool HasResource(ActorTag2D actor, ResourceType type)
        {
            DebugTool.Assert(
                _actorResources.ContainsKey(actor),
                $"Actor {actor.GetParent<Node2D>().Name} not found in resource manager."
            );
            return _actorResources[actor].GetValueOrDefault(type)?.Amount > 0;
        }

        /// <summary>
        /// Checks if a given actor is deficient in a specific resource.
        /// </summary>
        /// <param name="actor">The actor to check.</param>
        /// <param name="type">The type of resource to check for deficiency.</param>
        /// <returns>True if the actor is deficient in the resource, otherwise false.</returns>
        public bool IsDeficient(ActorTag2D actor, ResourceType type)
        {
            DebugTool.Assert(
                _actorResources.ContainsKey(actor),
                $"Actor {actor.GetParent<Node2D>().Name} not found in resource manager."
            );
            return _actorResources[actor].GetValueOrDefault(type)?.IsDeficient() ?? false;
        }

        /// <summary>
        /// Transfers resources from one actor to another.
        /// </summary>
        /// <param name="from">The actor to transfer resources from.</param>
        /// <param name="to">The actor to transfer resources to.</param>
        /// <param name="type">The type of resource to transfer.</param>
        /// <param name="amount">The amount of resource to transfer.</param>
        public void TranserResources(ActorTag2D from, ActorTag2D to, ResourceType type, float amount)
        {
            ResourceStat fromResource = _actorResources[from].GetValueOrDefault(type);
            ResourceStat toResource = _actorResources[to].GetValueOrDefault(type);

            if (fromResource == null || toResource == null) return;

            if (fromResource.Amount < amount)
            {
                amount = fromResource.Amount;
            }

            fromResource.Amount -= amount;
            toResource.Amount += amount;
        }

        public void ModifyResource(ActorTag2D actor, ResourceType type, float amount)
        {
            if (_actorResources[actor].TryGetValue(type, out ResourceStat resource))
            {
                resource.Amount += amount;
            }
        }
    }
}