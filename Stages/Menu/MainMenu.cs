using Godot;

namespace EmergentEchoes
{
	public partial class MainMenu : Node2D
	{
		private Sprite2D _titleTexture;
		private Button _startButton;
		private Button _exitButton;
		private AudioStreamPlayer _backgroundMusic;
		private PathFollow2D _pathFollow;

		private float _timeElapsed = 0;

		public override void _Ready()
		{
			_titleTexture = GetNode<Sprite2D>("MenuLayer/TitleSprite");
			_startButton = GetNode<Button>("MenuLayer/HBoxContainer/StartButton");
			_exitButton = GetNode<Button>("MenuLayer/HBoxContainer/ExitButton");

			_backgroundMusic = GetNode<AudioStreamPlayer>("BackgroundMusic");
			_backgroundMusic.Play();

			_pathFollow = GetNode<PathFollow2D>("Path2D/PathFollow2D");

			if (_startButton != null)
			{
				GD.Print("StartButton found");
				_startButton.Connect("pressed", Callable.From(OnStartButtonPressed));
			}
			else
			{
				GD.PrintErr("StartButton not found");
			}

			if (_exitButton != null)
			{
				GD.Print("ExitButton found");
				_exitButton.Connect("pressed", Callable.From(OnExitButtonPressed));
			}
			else
			{
				GD.PrintErr("ExitButton not found");
			}
		}

		public override void _Process(double delta)
		{
			_timeElapsed += (float)delta;

			_pathFollow.ProgressRatio += (float)(delta * 0.01);
			Vector2 title_pos = _titleTexture.GlobalPosition;
			title_pos.Y = 70 + 4 * Mathf.Sin(_timeElapsed);
			_titleTexture.GlobalPosition = title_pos;
		}

		private void OnStartButtonPressed()
		{
			_startButton.MouseFilter = Control.MouseFilterEnum.Ignore;
			_exitButton.MouseFilter = Control.MouseFilterEnum.Ignore;
			GetNode<AnimationPlayer>("AnimationPlayer").Play("fade_out");
		}

		private void OnExitButtonPressed()
		{
			GetTree().Quit();
		}

		private void ChangeSceneToOpening()
		{
			GetTree().ChangeSceneToFile("res://Stages/Island/world.tscn");
		}
	}
}
