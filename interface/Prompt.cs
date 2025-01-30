using Godot;
using System;

public partial class Prompt : CanvasLayer
{
	private Interactable _interactable;

	private void DisplayDescription()
    {
        this.Visible = !this.Visible;
        GD.Print(this.Visible ? "Showing Description" : "Hiding Description");

		// Pause or unpause the game
        Engine.TimeScale = this.Visible ? 0 : 1;
    }

	public override void _Ready()
	{
		_interactable = GetParent().GetNode<Interactable>("Interactable");

		_interactable.Interact = new Callable(this, nameof(DisplayDescription));
	}
}
