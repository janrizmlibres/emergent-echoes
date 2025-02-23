using Godot;

namespace NPCProcGen
{
    [Tool]
    public partial class PrisonMarker2D : Marker2D
    {
        [Export]
        public int Capacity { get; set; } = 1;
    }
}