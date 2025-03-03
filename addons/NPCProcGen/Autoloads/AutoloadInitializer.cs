using System.Collections.Generic;
using Godot;
using NPCProcGen.Core.Internal;

namespace NPCProcGen.Autoloads
{
    public static class AutoloadInitializer
    {
        public static void Init(Node currentScene)
        {
            List<ActorTag2D> actors = new();
            List<PrisonMarker2D> prisons = new();
            List<CropMarker2D> cropTiles = new();

            FindNodesRecursive(currentScene);

            Sensor.Initialize(actors, prisons, cropTiles);
            ResourceManager.Instance.Initialize(actors);

            void FindNodesRecursive(Node currentNode)
            {
                foreach (Node child in currentNode.GetChildren())
                {
                    switch (child)
                    {
                        case ActorTag2D actor:
                            actors.Add(actor);
                            break;
                        case PrisonMarker2D prison:
                            prisons.Add(prison);
                            break;
                        case CropMarker2D cropTile:
                            cropTiles.Add(cropTile);
                            break;
                    }

                    FindNodesRecursive(child);
                }
            }
        }
    }
}