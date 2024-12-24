using Godot;

namespace EmergentEchoes.Actors.NPCs.Traits
{
    public interface ITrait
    {
        float EvaluationAction();
        Vector2? GetTargetPosition();
        void OnInteract(Npc other);
        bool ShouldActivate();
    }
}