using Godot;
using System;

public partial class Rock : StaticBody2D
{
	private Interactable _interactable;
	private Sprite2D _brokenRock;
	private Sprite2D _fullRock;
	private Timer _regrowTimer;
	
	private void OnInteract()
	{
		_interactable.IsInteractable = false;
		_brokenRock.Show();
		_fullRock.Hide();
		_regrowTimer.Start();
	}
	
	private void OnRegrowTimerTimeout()
	{
		GD.Print("Tree Grown");
		_interactable.IsInteractable = true;
		_brokenRock.Hide();
		_fullRock.Show();
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_interactable = GetNode<Interactable>("Interactable"); 
		_brokenRock = GetNode<Sprite2D>("BrokenRock");
		_fullRock = GetNode<Sprite2D>("FullRock");
		_regrowTimer = GetNode<Timer>("RegrowTimer");

		_interactable.PrimaryInteract = new Callable(this, nameof(OnInteract));
	}
}
