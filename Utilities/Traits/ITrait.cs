using Godot;
using System;

namespace EmergentEchoes.Utilities.Traits
{
    public interface ITrait
    {
        public Tuple<string, float> EvaluateAction();
        public bool ShouldActivate();
    }
}