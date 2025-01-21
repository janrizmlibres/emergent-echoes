using Godot;
using System;

public partial class InteriorArea : Area2D
{
	private TileMapLayer _tileMapLayer;
	
	private void OnCharacterEntered(Node2D character)
	{
		if (character is not CharacterBody2D)
		{
			return;
		}
		
		_tileMapLayer.Hide();
	}
	
	private void OnCharacterExited(Node2D character)
	{
		
		if (character is not CharacterBody2D)
		{
			return;
		}
		
		_tileMapLayer.Show();
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_tileMapLayer = GetParent().GetNode<TileMapLayer>("Roof");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
