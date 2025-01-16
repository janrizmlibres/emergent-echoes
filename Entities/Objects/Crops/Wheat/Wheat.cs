using Godot;
using System;

public partial class Wheat : StaticBody2D
{
	private Interactable _interactable;
	private AnimatedSprite2D _wheatSprite;
	private Timer _regrowTimer;
	
	private void OnInteract()
	{
		_interactable.IsInteractable = false;
		_wheatSprite.SetFrame(0);
		_regrowTimer.Start();
	}

	private void OnPlantNextStage()
	{
		if (_wheatSprite.GetFrame() == 2)
		{
			_wheatSprite.SetFrame(_wheatSprite.GetFrame() + 1);
			_interactable.IsInteractable = true;
		}
		else
		{
			_wheatSprite.SetFrame(_wheatSprite.GetFrame() + 1);
			_regrowTimer.Start();
		}
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_interactable = GetNode<Interactable>("Interactable"); 
		_wheatSprite = GetNode<AnimatedSprite2D>("WheatSprite");
		_regrowTimer = GetNode<Timer>("RegrowTimer");
		
		_interactable.Interact = new Callable(this, nameof(OnInteract));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
