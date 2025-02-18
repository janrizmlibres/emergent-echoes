using System.Collections.Generic;
using Godot;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Autoloads
{
    /// <summary>
    /// Provides initialization methods for autoloaded singletons.
    /// </summary>
    public static class AutoloadInitializer
    {
        /// <summary>
        /// Initializes the WorldState and ResourceManager with actors found in the current scene.
        /// </summary>
        /// <param name="currentScene">The current scene node.</param>
        public static void Init(Node currentScene)
        {
            List<ActorTag2D> actors = new();
            FindActorsInNode(currentScene, actors);

            List<PrisonArea2D> prisons = new();
            FindPrisonsInNode(currentScene, prisons);

            Sensor.InitializeWorldState(actors, prisons);
            ResourceManager.Instance.Initialize(actors);

            // ! Remove temporary debug code in production
            // GD.Print("Actors in WorldState:");
            // foreach (ActorTag2D actor in WorldState.Instance.Actors)
            // {
            //     GD.Print(actor.Parent.Name);
            // }

            // ResourceManager.Instance.PrintActors();
        }

        /// <summary>
        /// Recursively finds all ActorTag2D nodes in the given node and its children.
        /// </summary>
        /// <param name="node">The node to search.</param>
        /// <param name="Actors">The list to populate with found ActorTag2D nodes.</param>
        private static void FindActorsInNode(Node node, List<ActorTag2D> Actors)
        {
            foreach (Node child in node.GetChildren())
            {
                if (child is ActorTag2D actor)
                {
                    Actors.Add(actor);
                }

                FindActorsInNode(child, Actors);
            }
        }

        private static void FindPrisonsInNode(Node node, List<PrisonArea2D> prisons)
        {
            foreach (Node child in node.GetChildren())
            {
                if (child is PrisonArea2D prison)
                {
                    prisons.Add(prison);
                }

                FindPrisonsInNode(child, prisons);
            }
        }
    }
}