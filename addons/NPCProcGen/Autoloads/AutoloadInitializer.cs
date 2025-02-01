using System.Collections.Generic;
using Godot;

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

            WorldState.Instance.Initialize(actors);
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
    }
}