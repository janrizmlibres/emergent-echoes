using Godot;

namespace EmergentEchoes.Entities.Hooks.CarryAnchor
{
	public partial class CarryAnchor : Marker2D
	{
		private Sprite2D _sprite2d;

		public override void _Ready()
		{
			ResourceLoader.Load("res://Entities/Hooks/CarryAnchor/CarryAnchor.tscn");
			_sprite2d = GetNode<Sprite2D>("Sprite2D");
		}

		public void SetTexture(string textureName)
		{
			CompressedTexture2D texture = GD.Load<CompressedTexture2D>(
				$"res://Entities/Hooks/CarryAnchor/Data/{textureName.ToLower()}_texture.tres"
			);
			_sprite2d.Texture = texture;
		}
	}
}