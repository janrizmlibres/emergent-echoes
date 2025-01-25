using Godot;
using System;

public partial class Chest : StaticBody2D
{
	[Export] public int Gold = 100;
	[Export] public int GoldAmount = 10;
	
	private Interactable _interactable;
	private ChestManager _chestManager;
	private CharacterBody2D _character;

	private void OnInteract()
	{
		// check if _character is not empty
		if (_character != null)
		{
			
		}
	}

	private void _on_chest_manager_body_entered(Node2D node)
	{
		if (node is CharacterBody2D character)
		{
			_character = character;
		}
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_chestManager = GetNode<ChestManager>("ChestManager");
		_interactable = GetNode<Interactable>("Interactable");
		
		_interactable.Interact = new Callable(this, nameof(OnInteract));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
