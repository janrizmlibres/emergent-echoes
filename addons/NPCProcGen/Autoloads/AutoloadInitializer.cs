using System.Collections.Generic;
using Godot;

namespace NPCProcGen.Autoloads
{
    public static class AutoloadInitializer
    {
        public static void Init(Node currentScene)
        {
            List<ActorTag2D> actors = new();
            FindActorsInNode(currentScene, actors);

            WorldState.Instance.Initialize(actors);
            ResourceManager.Instance.Initialize(actors);
        }

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