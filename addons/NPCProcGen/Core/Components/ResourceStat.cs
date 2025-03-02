using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;
using System;

namespace NPCProcGen.Core.Components
{
    public class ResourceStat
    {
        private const float TotalFoodLowerMultiplier = 2.5f;
        private const float TotalFoodUpperMultiplier = 5.5f;

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
                    ResourceType.Food => (5, 30),
                    ResourceType.Satiation => (5, 90),
                    ResourceType.Companionship => (10, 90),
                    ResourceType.Duty => (30, 70),
                    ResourceType.TotalFood => GetTotalFoodThresholds(),
                    ResourceType.None => (0, 0),
                    _ => throw new ArgumentOutOfRangeException(Type.ToString()),
                };
            }

            (int, int) GetTotalFoodThresholds()
            {
                int actorCount = Sensor.GetActorCount();
                LowerThreshold = (int)Math.Ceiling(actorCount * TotalFoodLowerMultiplier);
                UpperThreshold = (int)Math.Ceiling(actorCount * TotalFoodUpperMultiplier);
                return (LowerThreshold, UpperThreshold);
            }
        }

        public void Update(double delta)
        {
            Amount -= (float)(GetDecayRate() * delta);

            float GetDecayRate()
            {
                if (Type == ResourceType.Duty)
                    return Sensor.HasCrime() ? 0.1f : -0.1f;

                if (Type == ResourceType.Satiation || Type == ResourceType.Companionship)
                    return 0.1f;

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