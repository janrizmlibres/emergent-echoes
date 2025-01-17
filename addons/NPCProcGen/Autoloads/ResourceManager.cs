using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Autoloads
{
    public partial class ResourceManager : Node
    {
        private static readonly Lazy<ResourceManager> _instance = new(() => new ResourceManager());

        public static ResourceManager Instance { get { return _instance.Value; } }

        private ResourceManager() { }

        public List<ResourceType> TangibleTypes { get { return _tangibleTypes.ToList(); } }

        private readonly List<ResourceType> _tangibleTypes = new()
        {
            ResourceType.Money,
            ResourceType.Food
        };

        private readonly Dictionary<ActorTag2D, Dictionary<ResourceType, ResourceStat>> _actorResources = new();

        public void Initialize(List<ActorTag2D> actors)
        {
            foreach (ActorTag2D actor in actors)
            {
                Dictionary<ResourceType, ResourceStat> resources = new();

                if (actor is NPCAgent2D npc)
                {
                    resources[ResourceType.Money] = new(ResourceType.Money, npc.MoneyValue, npc.Money);
                    resources[ResourceType.Food] = new(ResourceType.Food, npc.FoodValue, npc.Food);
                    resources[ResourceType.Companionship] = new(ResourceType.Companionship,
                        npc.CompanionshipValue, npc.Companionship);
                }
                else
                {
                    resources[ResourceType.Money] = new ResourceStat(ResourceType.Money, actor.MoneyValue, 1);
                    resources[ResourceType.Food] = new ResourceStat(ResourceType.Food, actor.FoodValue, 1);
                    resources[ResourceType.Companionship] = new ResourceStat(ResourceType.Companionship, 100, 1);
                }

                _actorResources[actor] = resources;
            }
        }

        // public void PrintActors()
        // {
        //     GD.Print("Actors and Resources in ResourceManager:");
        //     foreach (ActorTag2D actor in _actorResources.Keys)
        //     {
        //         GD.Print(actor.Parent.Name);
        //         foreach (ResourceStat resource in _actorResources[actor].Values)
        //         {
        //             GD.Print(resource.Type.ToString() + ": " + resource.Amount);
        //         }
        //     }
        // }

        public ResourceStat GetResource(ActorTag2D actor, ResourceType type)
        {
            DebugTool.Assert(_actorResources.ContainsKey(actor),
                $"Actor {actor.Parent.Name} not found in resource manager.");
            return _actorResources[actor][type];
        }

        public bool HasResource(ActorTag2D actor, ResourceType type)
        {
            DebugTool.Assert(_actorResources.ContainsKey(actor),
                $"Actor {actor.Parent.Name} not found in resource manager.");
            return _actorResources[actor][type].Amount > 0;
        }

        public bool IsDeficient(ActorTag2D actor, ResourceType type)
        {
            DebugTool.Assert(_actorResources.ContainsKey(actor),
                $"Actor {actor.Parent.Name} not found in resource manager.");
            return _actorResources[actor][type].IsDeficient();
        }

        public void TranserResources(ActorTag2D from, ActorTag2D to, ResourceType type, float amount)
        {
            ResourceStat fromResource = _actorResources[from][type];
            ResourceStat toResource = _actorResources[to][type];

            if (fromResource.Amount < amount)
            {
                amount = fromResource.Amount;
            }

            fromResource.Amount -= amount;
            toResource.Amount += amount;

            GD.Print("Transferred " + amount + " " + type.ToString() + " from " + from.Parent.Name
                + " to " + to.Parent.Name);
        }
    }
}