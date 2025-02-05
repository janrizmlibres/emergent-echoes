using EmergentEchoes.Entities.UI.Indicators;
using Godot;

namespace EmergentEchoes.Utilities.Game
{
	public partial class FloatTextController : Node2D
	{
		private const float Duration = 2;
		private const float Spread = Mathf.Pi / 2;

		private readonly static PackedScene _floatingTextScene =
			GD.Load<PackedScene>("res://Entities/UI/Indicators/Floating Text/floating_text.tscn");

		private static Vector2 _travel = new(0, -10);

		public void ShowFloatText(string value)
		{
			FloatingText floatingText = (FloatingText)_floatingTextScene.Instantiate();

			Vector2 newPosition = floatingText.GlobalPosition;
			newPosition.X -= floatingText.Size.X / 2;
			newPosition.Y -= 24;
			floatingText.GlobalPosition = newPosition;

			AddChild(floatingText);
			floatingText.ShowValue(value, _travel, Duration, Spread);
		}
	}
}