using Godot;
using System;

namespace EmergentEchoes
{
	public partial class MainMenu : Control
	{
		private Button _startButton;
		private Button _exitButton;
		private CheckButton _musicController;
		private AudioStreamPlayer _backgroundMusic;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			_startButton = GetNode<Button>("HBoxContainer/StartButton");
			_exitButton = GetNode<Button>("HBoxContainer/ExitButton");
			_musicController = GetNode<CheckButton>("MusicController");
			_backgroundMusic = GetNode<AudioStreamPlayer>("BackgroundMusic");

			if (_startButton != null)
			{
				GD.Print("StartButton found");
				_startButton.Connect("pressed", Callable.From(_OnStartButtonPressed));
			}
			else
			{
				GD.PrintErr("StartButton not found");
			}

			if (_exitButton != null)
			{
				GD.Print("ExitButton found");
				_exitButton.Connect("pressed", Callable.From(_OnExitButtonPressed));
			}
			else
			{
				GD.PrintErr("ExitButton not found");
			}

			if (_musicController != null)
			{
				GD.Print("MusicController found");
				_musicController.Connect("toggled", Callable.From<bool>(_OnMusicControllerToggled));
			}
			else
			{
				GD.PrintErr("MusicController not found");
			}
		}

		private void _OnStartButtonPressed()
		{
			GD.Print("Start button pressed");
			GetTree().ChangeSceneToFile("res://world.tscn");
		}

		private void _OnExitButtonPressed()
		{
			GD.Print("Exit button pressed");
			GetTree().Quit();
		}

		private void _OnMusicControllerToggled(bool buttonPressed)
		{
			if (buttonPressed)
			{
				GD.Print("Music enabled");
				_backgroundMusic.Play();
			}
			else
			{
				GD.Print("Music disabled");
				_backgroundMusic.Stop();
			}
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}
	}
}
