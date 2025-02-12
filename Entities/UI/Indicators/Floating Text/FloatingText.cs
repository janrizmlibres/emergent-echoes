using System.Collections.Generic;
using Godot;
using NPCProcGen.Core.Components.Enums;

namespace EmergentEchoes.Entities.UI.Indicators
{
	public partial class FloatingText : Control
	{
		private readonly Dictionary<ResourceType, CompressedTexture2D> iconTextures = new()
		{
			{
				ResourceType.Money,
				GD.Load<CompressedTexture2D>(
					"res://Entities/UI/Indicators/Floating Text/Data/icon_compressed_texture_2d.tres"
				)
			},
			{
				ResourceType.Satiation,
				GD.Load<CompressedTexture2D>(
					"res://Entities/UI/Indicators/Floating Text/Data/cutlery_compressed_texture_2d.tres"
				)
			},
			{
				ResourceType.Food,
				GD.Load<CompressedTexture2D>(
					"res://Entities/UI/Indicators/Floating Text/Data/wheat_compressed_texture_2d.tres"
				)
			},
			{
				ResourceType.Companionship,
				GD.Load<CompressedTexture2D>(
					"res://Entities/UI/Indicators/Floating Text/Data/team_compressed_texture_2d.tres"
				)
			}
		};

		private readonly LabelSettings _normalLabelSettings = GD.Load<LabelSettings>(
			"res://Entities/UI/Indicators/Floating Text/Data/normal_float_text_label_settings.tres"
		);

		private readonly LabelSettings _negativeLabelSettings = GD.Load<LabelSettings>(
			"res://Entities/UI/Indicators/Floating Text/Data/negative_float_text_label_settings.tres"
		);

		private TextureRect _textureRect;
		private Label _label;

		public override void _Ready()
		{
			_textureRect = GetNode<TextureRect>("TextureRect");
			_label = GetNode<Label>("Label");
		}

		public async void ShowValue(ResourceType type, string value, bool isNormal,
			Vector2 travel, float duration, float spread)
		{
			_textureRect.Texture = iconTextures[type];
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