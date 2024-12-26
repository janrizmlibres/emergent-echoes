using Godot;
using System;

namespace EmergentEchoes
{
    public sealed partial class ResourceManager : Node
    {
        private static readonly Lazy<ResourceManager> _lazy = new(() => new ResourceManager());
        public static ResourceManager Instance { get { return _lazy.Value; } }

        private ResourceManager() { }
    }
}