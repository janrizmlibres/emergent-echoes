using Godot;

namespace EmergentEchoes.Entities.UI.Indicators
{
	public partial class FloatingText : Control
	{
		private TextureRect _textureRect;
		private Label _label;

		public override void _Ready()
		{
			_textureRect = GetNode<TextureRect>("TextureRect");
			_label = GetNode<Label>("Label");
		}

		public async void ShowValue(string value, Vector2 travel, float duration, float spread)
		{
			_label.Text = value;
			Vector2 movement = travel.Rotated((float)GD.RandRange(-spread / 2, spread / 2));
			PivotOffset = Size / 2;

			Tween tween = CreateTween().SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut).SetParallel();
			tween.TweenProperty(this, "position", Position + movement, duration);

			await ToSignal(tween, "finished");
			QueueFree();
		}
	}
}