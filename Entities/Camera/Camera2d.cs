using Godot;

namespace EmergentEchoes.Entities.Camera
{
	public partial class Camera2d : Camera2D
	{
		public override void _Ready()
		{
			Marker2D topLeft = GetNode<Marker2D>("TopLeft");
			Marker2D bottomRight = GetNode<Marker2D>("BottomRight");

			
		}
	}
}
