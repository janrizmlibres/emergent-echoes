using Godot;

namespace EmergentEchoes.Entities.Hooks.CarryProp
{
	public partial class CarryProp : Node2D
	{
		private Sprite2D _sprite2d;

		public override void _Ready()
		{
			_sprite2d = GetNode<Sprite2D>("Sprite2D");
		}

		public void SetTexture(string actorName)
		{
			CompressedTexture2D texture = GD.Load<CompressedTexture2D>(
				$"res://Entities/Actors/Data/{actorName.ToLower()}_texture.tres"
			);
			_sprite2d.Texture = texture;
		}
	}
}