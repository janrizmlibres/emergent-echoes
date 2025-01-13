using Godot;
using System;

public partial class Door : Area2D
{
	private Sprite2D _closeDoor;
	private Sprite2D _openDoor;
	private void OnCharacterEntered(Node2D character)
	{
		if (character is not CharacterBody2D)
		{
			return;
		}
		_closeDoor.Hide();
		_openDoor.Show();
	}
	
	private void OnCharacterExited(Node2D character)
	{
		
		if (character is not CharacterBody2D)
		{
			return;
		}
		
		_closeDoor.Show();
		_openDoor.Hide();
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_closeDoor = GetNode<Sprite2D>("CloseDoor");
		_openDoor = GetNode<Sprite2D>("OpenDoor");
		
		_closeDoor.Show();
		_openDoor.Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
