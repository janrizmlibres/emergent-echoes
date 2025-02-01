using Godot;
using NPCProcGen.Core.Components.Enums;

namespace NPCProcGen.Core.Components.Variants
{
    /// <summary>
    /// Represents data related to a theft event.
    /// </summary>
    public partial class TheftData : GodotObject
    {
        /// <summary>
        /// Gets or sets the type of resource that was stolen.
        /// </summary>
        public ResourceType ResourceType { get; set; }

        /// <summary>
        /// Gets or sets the amount of resource that was stolen.
        /// </summary>
        public int Amount { get; set; }
    }
}