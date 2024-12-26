using Godot;
using System;

namespace EmergentEchoes.Utilities.Traits
{
    public interface ITrait
    {
        public Tuple<float, string> EvaluateAction();
        public bool ShouldActivate();
    }
}