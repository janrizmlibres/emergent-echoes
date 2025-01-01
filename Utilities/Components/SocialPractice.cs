using Godot;
using System;

namespace EmergentEchoes.Utilities.Components
{
    public class SocialPractice
    {
        public enum Practice
        {
            Proactive,
            RefusedPetition,
            FailedPetition,
        }

        public Practice Type { get; set; }
    }
}