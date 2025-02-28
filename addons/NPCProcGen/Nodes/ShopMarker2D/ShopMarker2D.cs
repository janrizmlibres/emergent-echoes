using Godot;

namespace NPCProcGen
{
    [Tool]
    public partial class ShopMarker2D : Marker2D
    {
        [Export]
        public Vector2 Direction { get; set; } = Vector2.Up;
    }
}