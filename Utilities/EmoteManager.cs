using System.Diagnostics;
using EmergentEchoes.Entities.UI.Indicators;
using Godot;
using NPCProcGen.Core.Helpers;

namespace EmergentEchoes.Utilities
{
    public static class EmoteManager
    {
        private readonly static PackedScene _emoteBubbleScene =
            GD.Load<PackedScene>("res://Entities/UI/Indicators/Emote Bubbles/emote_bubble.tscn");
        private readonly static AnimationLibrary _exclamationAnim =
            GD.Load<AnimationLibrary>(
                "res://Entities/UI/Indicators/Emote Bubbles/Data/exclamation_anim.res"
            );

        public static void ShowEmoteBubble(Node2D character)
        {
            EmoteBubble emoteBubble = (EmoteBubble)_emoteBubbleScene.Instantiate();
            emoteBubble.SetAnimationLibrary(_exclamationAnim);

            Vector2 newPosition = emoteBubble.Position;
            newPosition.Y -= 12;
            emoteBubble.Position = newPosition;

            character.AddChild(emoteBubble);
        }
    }
}