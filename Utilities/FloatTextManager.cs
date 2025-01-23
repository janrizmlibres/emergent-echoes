using EmergentEchoes.Entities.UI.Indicators;
using Godot;

namespace EmergentEchoes.Utilities
{
	public static class FloatTextManager
	{
		private readonly static PackedScene _floatingTextScene =
			GD.Load<PackedScene>("res://Entities/UI/Indicators/Floating Text/floating_text.tscn");

		private static Vector2 _travel = new(0, -10);
		private static readonly float _duration = 2;
		private static readonly float _spread = Mathf.Pi / 2;

		public static void ShowFloatText(Node2D character, string value)
		{
			FloatingText floatingText = (FloatingText)_floatingTextScene.Instantiate();

			Vector2 newPosition = floatingText.GlobalPosition;
			newPosition.X -= floatingText.Size.X / 2;
			newPosition.Y -= 24;
			floatingText.GlobalPosition = newPosition;

			character.AddChild(floatingText);
			floatingText.ShowValue(value, _travel, _duration, _spread);
		}
	}
}