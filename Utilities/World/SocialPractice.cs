using Godot;
using System;

namespace EmergentEchoes.Utilities.World
{
    public class SocialPractice
    {
        public enum Practice
        {
            Proactive,
            RefusedPetition,
            FailedPetition,
        }

        public Practice PracticeType { get; set; }
    }
}