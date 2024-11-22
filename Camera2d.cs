using Godot;
using System;

public partial class Camera2d : Camera2D
{
	public override void _Ready()
	{
		Marker2D topLeft = GetNode<Marker2D>("TopLeft");
		Marker2D bottomRight = GetNode<Marker2D>("BottomRight");

		LimitTop = (int)topLeft.GlobalPosition.Y;
		LimitLeft = (int)topLeft.GlobalPosition.X;
		LimitBottom = (int)bottomRight.GlobalPosition.Y;
		LimitRight = (int)bottomRight.GlobalPosition.X;
	}
}
