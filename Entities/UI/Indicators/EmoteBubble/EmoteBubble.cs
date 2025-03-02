using Godot;
using System;

namespace EmergentEchoes.Entities.UI.Indicators
{
	public partial class EmoteBubble : Node2D
	{
		public string AnimationName { get; set; }

		private AnimationPlayer _animationPlayer;
		private Timer _durationtTimer;

		public override void _Ready()
		{
			_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
			_animationPlayer.AnimationFinished += OnAnimationFinished;

			_durationtTimer = GetNode<Timer>("DurationTimer");
			_durationtTimer.Timeout += OnDurationTimerTimeout;

			_animationPlayer.Play($"{AnimationName}/inflate");
		}

		private void OnAnimationFinished(StringName animName)
		{
			if (animName.ToString().Contains("inflate"))
			{
				_durationtTimer.Start();
			}
			else if (animName.ToString().Contains("deflate"))
			{
				QueueFree();
			}
			else
			{
				throw new ArgumentException("Invalid animation name");
			}
		}

		private void OnDurationTimerTimeout()
		{
			_animationPlayer.Play($"{AnimationName}/deflate");
		}
	}
}