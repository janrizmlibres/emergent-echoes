using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;
using System;

namespace NPCProcGen.Core.Components
{
    public class ResourceStat
    {
        private const int FoodLowerThreshold = 5;
        private const int FoodUpperThreshold = 20;

        public ResourceType Type { get; private set; }

        private float _amount;
        public float Amount
        {
            get => _amount;
            set
            {
                _amount = Math.Clamp(value, 0, GetMaxValue());
                _amount = (float)(IsInteger() ? Math.Floor(_amount) : _amount);

                bool IsInteger()
                {
                    return Type == ResourceType.Money || Type == ResourceType.Food
                        || Type == ResourceType.TotalFood;
                }
            }
        }

        public float Weight { get; private set; }

        public int LowerThreshold { get; private set; }

        public int UpperThreshold { get; private set; }

        public ResourceStat(ResourceType type, float amount, float weight)
        {
            Type = type;
            Amount = amount;
            Weight = weight;

            (LowerThreshold, UpperThreshold) = GetThresholdValues();

            (int, int) GetThresholdValues()
            {
                return Type switch
                {
                    ResourceType.Money => (20, 1000),
                    ResourceType.Food => (FoodLowerThreshold, FoodUpperThreshold),
                    ResourceType.Satiation => (5, 90),
                    ResourceType.Companionship => (10, 90),
                    ResourceType.Duty => (30, 90),
                    ResourceType.TotalFood => GetTotalFoodThresholds(),
                    ResourceType.None => (0, 0),
                    _ => throw new ArgumentOutOfRangeException(Type.ToString()),
                };
            }

            (int, int) GetTotalFoodThresholds()
            {
                int actorCount = Sensor.GetActorCount();
                LowerThreshold = (int)MathF.Ceiling(actorCount * FoodLowerThreshold);
                UpperThreshold = (int)MathF.Ceiling(actorCount * FoodUpperThreshold);
                return (LowerThreshold, UpperThreshold);
            }
        }

        public void Update(double delta)
        {
            Amount -= GetDecayRate() * (float)delta;

            float GetDecayRate()
            {
                if (Type == ResourceType.Duty)
                    return Sensor.HasCrime() ? 0.01f : -0.01f;

                if (Type == ResourceType.Satiation || Type == ResourceType.Companionship)
                    return 0.01f;

                return 0;
            }
        }

        public int GetMinRaise()
        {
            return Type == ResourceType.Money ? 10 : 1;
        }

        public float GetMaxValue()
        {
            return Type switch
            {
                ResourceType.None => 0,
                ResourceType.Money => 10000,
                ResourceType.Food => 100,
                ResourceType.Satiation => 100,
                ResourceType.Companionship => 100,
                ResourceType.Duty => 100,
                ResourceType.TotalFood => float.MaxValue,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        public bool IsDeficient()
        {
            return Amount < LowerThreshold;
        }

        public bool IsUnbounded()
        {
            return Type == ResourceType.TotalFood;
        }
    }
}