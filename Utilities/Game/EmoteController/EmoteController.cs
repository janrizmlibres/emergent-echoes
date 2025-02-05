using System.Linq;
using EmergentEchoes.Entities.UI.Indicators;
using EmergentEchoes.Utilities.Game.Enums;
using Godot;

namespace EmergentEchoes.Utilities.Game
{
    public partial class EmoteController : Node2D
    {
        private const float MinBubbleInterval = 2;
        private const float MaxBubbleInterval = 5;

        private readonly static PackedScene _emoteBubbleScene =
            GD.Load<PackedScene>("res://Entities/UI/Indicators/Emote Bubble/emote_bubble.tscn");

        private EmoteBubble _emoteBubble;

        private Timer _timer;

        public override void _Ready()
        {
            _timer = GetNode<Timer>("Timer");
            _timer.Timeout += OnTimerTimeout;
        }

        public void Activate()
        {
            ShowRandomEmote();
            _timer.Start(GD.RandRange(MinBubbleInterval, MaxBubbleInterval));
        }

        public void Deactivate()
        {
            _timer.Stop();
        }

        public void ShowEmoteBubble(Emote emote)
        {
            EmoteBubble emoteBubble = (EmoteBubble)_emoteBubbleScene.Instantiate();
            emoteBubble.AnimationName = emote.ToString().ToLower();

            Vector2 newPosition = emoteBubble.Position;
            newPosition.Y -= 15;
            emoteBubble.Position = newPosition;

            if (IsInstanceValid(_emoteBubble))
            {
                _emoteBubble.QueueFree();
            }

            _emoteBubble = emoteBubble;
            AddChild(emoteBubble);
        }

        private void ShowRandomEmote()
        {
            Emote emote = CoreHelpers.ShuffleEnum<Emote>().First();
            ShowEmoteBubble(emote);
        }

        private void OnTimerTimeout()
        {
            ShowRandomEmote();
        }
    }
}