using EmergentEchoes.Entities.UI.Indicators;
using EmergentEchoes.Utilities.Enums;
using Godot;

namespace EmergentEchoes.Utilities
{
    public static class EmoteManager
    {
        private readonly static PackedScene _emoteBubbleScene =
            GD.Load<PackedScene>("res://Entities/UI/Indicators/Emote Bubble/emote_bubble.tscn");

        public static void ShowEmoteBubble(Node2D character, Emote emote)
        {
            EmoteBubble emoteBubble = (EmoteBubble)_emoteBubbleScene.Instantiate();
            emoteBubble.AnimationName = emote.ToString().ToLower();

            Vector2 newPosition = emoteBubble.Position;
            newPosition.Y -= 15;
            emoteBubble.Position = newPosition;

            character.AddChild(emoteBubble);
        }
    }
}