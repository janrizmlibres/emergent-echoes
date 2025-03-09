using Godot;
using NPCProcGen;

namespace EmergentEchoes.Entities.Objects.Crop
{
    public partial class Crop : Sprite2D
    {
        private CropMarker2D _cropMarker;

        public override void _Ready()
        {
            _cropMarker = GetParent<CropMarker2D>();
        }

        public void Update()
        {
            Frame = _cropMarker.GetGrowthProgress() switch
            {
                >= 0.9f => 10,
                >= 0.6f => 9,
                >= 0.3f => 8,
                _ => 7
            };
        }
    }
}