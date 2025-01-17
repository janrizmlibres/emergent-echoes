using Godot;
using NPCProcGen.Core.Helpers;
using System;

namespace EmergentEchoes.Entities.UI.Indicators
{
	public partial class EmoteBubble : Node2D
	{
		private AnimationPlayer _animationPlayer;
		private Timer _durationtTimer;

		public override void _Ready()
		{
			_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
			_animationPlayer.AnimationFinished += OnAnimationFinished;

			_durationtTimer = GetNode<Timer>("DurationTimer");
			_durationtTimer.Timeout += OnDurationTimerTimeout;

			_animationPlayer.Play("emote/inflate");
		}

		public void SetAnimationLibrary(AnimationLibrary animLib)
		{
			_animationPlayer ??= GetNode<AnimationPlayer>("AnimationPlayer");
			_animationPlayer.AddAnimationLibrary("emote", animLib);
		}

		private void OnAnimationFinished(StringName animName)
		{
			switch (animName)
			{
				case "emote/inflate":
					_durationtTimer.Start();
					break;
				case "emote/deflate":
					QueueFree();
					break;
				default:
					throw new ArgumentException($"Unknown animation name: {animName}");
			}
		}

		private void OnDurationTimerTimeout()
		{
			_animationPlayer.Play("emote/deflate");
		}
	}
}