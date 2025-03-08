    using Godot;

namespace NPCProcGen
{
    // Attended must be another variable because it is not a state
    public enum CropStatus
    {
        Dormant,
        Attended,
        Growing,
        Grown
    }

    [Tool]
    public partial class CropMarker2D : Marker2D
    {
        private const float GrowthDuration = 60;

        private CropStatus _status = CropStatus.Dormant;
        public CropStatus Status
        {
            get => _status;
            set
            {
                _status = value;

                if (value == CropStatus.Dormant)
                {
                    _growthTimer = 0;
                }
            }
        }

        private float _growthTimer = 0;

        public void Update(double delta)
        {
            if (Status != CropStatus.Growing) return;

            _growthTimer += (float)delta;

            if (_growthTimer > GrowthDuration)
            {
                Status = CropStatus.Grown;
                _growthTimer = GrowthDuration;
            }
        }

        public float GetGrowthProgress()
        {
            return _growthTimer / GrowthDuration;
        }
    }
}