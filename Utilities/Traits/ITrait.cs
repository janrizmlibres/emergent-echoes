using EmergentEchoes.addons.NPCNode;
using Godot;

namespace EmergentEchoes.Utilities.Traits
{
    public interface ITrait
    {
        float EvaluationAction();
        Vector2? GetTargetPosition();
        void OnInteract(NPC2D other);
        bool ShouldActivate();
    }
}