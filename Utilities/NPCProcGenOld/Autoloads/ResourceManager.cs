using System;
using System.Collections.Generic;
using System.Linq;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Traits;

namespace NPCProcGen.Autoloads
{
    using ResourceDict = Dictionary<ActorTag2D, Dictionary<ResourceType, ResourceStat>>;

    public sealed class ResourceManager
    {
        private static readonly Lazy<ResourceManager> _instance = new(() => new ResourceManager());

        public static ResourceManager Instance => _instance.Value;

        private ResourceManager() { }

        private readonly ResourceType[] _tangibleTypes = new[]
        {
            ResourceType.Money,
            ResourceType.Food
        };
        public List<ResourceType> TangibleTypes => _tangibleTypes.ToList();

        private readonly ResourceDict _actorResources = new();
        private ResourceStat _totalFood;

        public void Initialize(List<ActorTag2D> actors)
        {
            _totalFood = new ResourceStat(ResourceType.TotalFood, 1, 1);

            foreach (ActorTag2D actor in actors)
            {
                Dictionary<ResourceType, ResourceStat> resources = new();
                _totalFood.Amount += actor.FoodAmount;

                if (actor.IsPlayer())
                {
                    AddResource(ResourceType.Money, actor.MoneyAmount, 1);
                    AddResource(ResourceType.Food, actor.FoodAmount, 1);
                }
                else
                {
                    NPCAgent2D npc = actor as NPCAgent2D;

                    AddResource(ResourceType.Money, npc.MoneyAmount, npc.Money);
                    AddResource(ResourceType.Food, npc.FoodAmount, npc.Food);
                    AddResource(ResourceType.Satiation, npc.SatiationAmount, npc.Satiation);
                    AddResource(
                        ResourceType.Companionship,
                        npc.CompanionshipAmount,
                        npc.Companionship
                    );

                    if (npc.Traits.Any(t => t is LawfulTrait))
                    {
                        AddResource(ResourceType.Duty, 100, 1);
                    }
                }

                _actorResources[actor] = resources;

                void AddResource(ResourceType type, float value, float weight)
                {
                    resources[type] = new ResourceStat(type, value, weight);
                }
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

        public ResourceStat GetResource(ResourceType type, ActorTag2D actor)
        {
            if (type == ResourceType.TotalFood) return _totalFood;
            if (actor == null) throw new ArgumentNullException(nameof(actor));
            return _actorResources[actor].GetValueOrDefault(type);
        }

        public float GetResourceAmount(ResourceType type, ActorTag2D actor)
        {
            return GetResource(type, actor)?.Amount ?? 0;
        }

        public bool HasResource(ResourceType type, ActorTag2D actor)
        {
            return GetResourceAmount(type, actor) > 0;
        }

        public bool IsDeficient(ResourceType type, ActorTag2D actor)
        {
            return GetResource(type, actor)?.IsDeficient() ?? false;
        }

        public void ModifyResource(ResourceType type, float amount, ActorTag2D actor)
        {
            if (type == ResourceType.TotalFood)
            {
                _totalFood.Amount += amount;
                return;
            }

            if (actor == null) throw new ArgumentNullException(nameof(actor));

            if (_actorResources[actor].TryGetValue(type, out ResourceStat resource))
            {
                resource.Amount += amount;
            }
        }

        public void TranserResources(ActorTag2D from, ActorTag2D to, ResourceType type, float amount)
        {
            if (type == ResourceType.TotalFood)
            {
                throw new ArgumentException("Cannot transfer total food");
            }

            ResourceStat fromResource = GetResource(type, from);
            ResourceStat toResource = GetResource(type, to);

            if (fromResource == null || toResource == null) return;

            fromResource.Amount -= amount;
            toResource.Amount += amount;
        }
    }
}