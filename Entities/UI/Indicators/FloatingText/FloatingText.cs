using Godot;
using NPCProcGen.Core.Components.Enums;

namespace EmergentEchoes.Entities.UI.Indicators
{
	public partial class FloatingText : Control
	{
		private LabelSettings _normalLabelSettings;
		private LabelSettings _negativeLabelSettings;

		private TextureRect _textureRect;
		private Label _label;

		public override void _Ready()
		{
			_textureRect = GetNode<TextureRect>("TextureRect");
			_label = GetNode<Label>("Label");

			_normalLabelSettings = ResourceLoader.Load<LabelSettings>(
				"res://Entities/UI/Indicators/FloatingText/Data/normal_label.tres"
			);
			_negativeLabelSettings = ResourceLoader.Load<LabelSettings>(
				"res://Entities/UI/Indicators/FloatingText/Data/negative_label.tres"
			);
		}

		public async void ShowValue(ResourceType type, string value, bool isNormal,
			Vector2 travel, float duration, float spread)
		{
			Texture2D texture = ResourceLoader.Load<Texture2D>(
				$"res://Entities/UI/Indicators/FloatingText/Art/{type.ToString().ToLower()}.png"
			);

			_textureRect.Texture = texture;
			_label.Text = value;
			_label.LabelSettings = isNormal ? _normalLabelSettings : _negativeLabelSettings;

			Vector2 movement = travel.Rotated((float)GD.RandRange(-spread / 2, spread / 2));
			PivotOffset = Size / 2;

			Tween tween = CreateTween().SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut).SetParallel();
			tween.TweenProperty(this, "position", Position + movement, duration);

			await ToSignal(tween, "finished");
			QueueFree();
		}
	}
}