using Godot;

namespace NPCProcGen
{
    [Tool]
    public partial class CropMarker2D : Marker2D
    {
        [Export]
        public NPCAgent2D AssignedAgent { get; set; }

        public bool IsReady { get; set; } = false;
    }
}