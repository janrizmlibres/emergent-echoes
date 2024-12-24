using Godot;

namespace EmergentEchoes.Utilities.Traits
{
    public interface ITrait
    {
        float EvaluationAction();
        Vector2? GetTargetPosition();
        void OnInteract(Npc other);
        bool ShouldActivate();
    }
}