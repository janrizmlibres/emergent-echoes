using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Autoloads
{
    /// <summary>
    /// Manages resources for actors in the game.
    /// </summary>
    public partial class ResourceManager : Node
    {
        private static readonly Lazy<ResourceManager> _instance = new(() => new ResourceManager());

        /// <summary>
        /// Gets the singleton instance of the ResourceManager.
        /// </summary>
        public static ResourceManager Instance { get { return _instance.Value; } }

        private ResourceManager() { }

        /// <summary>
        /// Gets a list of tangible resource types.
        /// </summary>
        public List<ResourceType> TangibleTypes { get { return _tangibleTypes.ToList(); } }

        /// <summary>
        /// List of tangible resource types.
        /// </summary>
        private readonly List<ResourceType> _tangibleTypes = new()
        {
            ResourceType.Money,
            ResourceType.Satiation
        };

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
                        npc.MoneyValue, npc.Money);

                    resources[ResourceType.Satiation] = new ResourceStat(ResourceType.Satiation,
                        npc.SatiationValue, npc.Satiation);

                    resources[ResourceType.Companionship] = new ResourceStat(ResourceType.Companionship,
                        npc.CompanionshipValue, npc.Companionship);
                }
                else
                {
                    resources[ResourceType.Money] = new ResourceStat(
                        ResourceType.Money, actor.MoneyValue, 1);

                    resources[ResourceType.Satiation] = new ResourceStat(
                        ResourceType.Satiation, 100, 1);

                    resources[ResourceType.Companionship] = new ResourceStat(
                        ResourceType.Companionship, 100, 1);
                }

                _actorResources[actor] = resources;
            }
        }

        /// <summary>
        /// Gets the resource of a specific type for a given actor.
        /// </summary>
        /// <param name="actor">The actor to get the resource for.</param>
        /// <param name="type">The type of resource to get.</param>
        /// <returns>The resource stat of the specified type for the actor.</returns>
        public ResourceStat GetResource(ActorTag2D actor, ResourceType type)
        {
            DebugTool.Assert(_actorResources.ContainsKey(actor),
                $"Actor {actor.Parent.Name} not found in resource manager.");
            return _actorResources[actor][type];
        }

        /// <summary>
        /// Checks if a given actor has a specific resource.
        /// </summary>
        /// <param name="actor">The actor to check.</param>
        /// <param name="type">The type of resource to check for.</param>
        /// <returns>True if the actor has the resource, otherwise false.</returns>
        public bool HasResource(ActorTag2D actor, ResourceType type)
        {
            DebugTool.Assert(_actorResources.ContainsKey(actor),
                $"Actor {actor.Parent.Name} not found in resource manager.");
            return _actorResources[actor][type].Amount > 0;
        }

        /// <summary>
        /// Checks if a given actor is deficient in a specific resource.
        /// </summary>
        /// <param name="actor">The actor to check.</param>
        /// <param name="type">The type of resource to check for deficiency.</param>
        /// <returns>True if the actor is deficient in the resource, otherwise false.</returns>
        public bool IsDeficient(ActorTag2D actor, ResourceType type)
        {
            DebugTool.Assert(_actorResources.ContainsKey(actor),
                $"Actor {actor.Parent.Name} not found in resource manager.");
            return _actorResources[actor][type].IsDeficient();
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
        
        /// <summary>
        /// Adds money to the specified actor.
        /// </summary>
        /// <param name="actor">The actor to add money to.</param>
        /// <param name="amount">The amount of money to add.</param>
        public void AddMoney(ActorTag2D actor, float amount)
        {
            if (_actorResources.TryGetValue(actor, out var resources))
            {
                resources[ResourceType.Money].Amount += amount;
            }
            else
            {
                GD.PrintErr($"Actor {actor.Parent.Name} not found in resource manager.");
            }
        }
        
        /// <summary>
        /// Deducts money from the specified actor.
        /// </summary>
        /// <param name="actor">The actor to deduct money from.</param>
        /// <param name="amount">The amount of money to deduct.</param>
        public bool DeductMoney(ActorTag2D actor, float amount)
        {
            if (_actorResources.TryGetValue(actor, out var resources))
            {
                if (resources[ResourceType.Money].Amount > 0)
                {
                    resources[ResourceType.Money].Amount -= amount;
                    return true;
                }
                else
                {
                    GD.PrintErr($"Actor {actor.Parent.Name} has no money to deduct.");
                    return false;
                }
            }
            else
            {
                GD.PrintErr($"Actor {actor.Parent.Name} not found in resource manager.");
                return false;
            }
        }
    }
}