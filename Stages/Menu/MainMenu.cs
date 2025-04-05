using Godot;
using System;

namespace EmergentEchoes
{
	public partial class MainMenu : Control
	{
		private Button _startButton;
		private Button _exitButton;
		private Button _musicController;
		private AudioStreamPlayer _backgroundMusic;
		private bool _isMusicPlaying = false;

		private Texture2D _musicEnabledIcon;
		private Texture2D _musicDisabledIcon;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			_startButton = GetNode<Button>("HBoxContainer/StartButton");
			_exitButton = GetNode<Button>("HBoxContainer/ExitButton");
			_musicController = GetNode<Button>("MusicController");
			_backgroundMusic = GetNode<AudioStreamPlayer>("BackgroundMusic");
		
			_musicEnabledIcon = GD.Load<Texture2D>("res://Assets/Button/Rect-Medium/34_Music (3)Cyan.png");
			_musicDisabledIcon = GD.Load<Texture2D>("res://Assets/Button/Rect-Medium/33_Music_Disable (3)Cyan.png");

			// Resize the icons
			_musicEnabledIcon = ResizeTexture(_musicEnabledIcon, 15, 15);
			_musicDisabledIcon = ResizeTexture(_musicDisabledIcon, 15, 15);

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
				_musicController.Connect("pressed", Callable.From(_OnMusicControllerPressed));
				_musicController.Icon = _musicDisabledIcon; // Set initial icon
			}
			else
			{
				GD.PrintErr("MusicController not found");
			}
		}

		private Texture2D ResizeTexture(Texture2D texture, int width, int height)
		{
			Image img = texture.GetImage();
			img.Resize(width, height);
			Texture2D resizedTexture = ImageTexture.CreateFromImage(img);
			return resizedTexture;
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

		private void _OnMusicControllerPressed()
		{
			_isMusicPlaying = !_isMusicPlaying;
			if (_isMusicPlaying)
			{
				GD.Print("Music enabled");
				_backgroundMusic.Play();
				_musicController.Icon = _musicEnabledIcon;
			}
			else
			{
				GD.Print("Music disabled");
				_backgroundMusic.Stop();
				_musicController.Icon = _musicDisabledIcon;
			}
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}
	}
}
