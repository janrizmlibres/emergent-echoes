using Godot;

namespace NPCProcGen
{
    [Tool]
    public partial class ShopMarker2D : Marker2D
    {
        [Export(PropertyHint.Enum, "Up,Down,Left,Right")]
        public int Direction { get; set; } = 0;

        public Vector2 GetDirection()
        {
            return Direction switch
            {
                0 => Vector2.Up,
                1 => Vector2.Down,
                2 => Vector2.Left,
                3 => Vector2.Right,
                _ => throw new System.NotImplementedException()
            };
        }
    }
}