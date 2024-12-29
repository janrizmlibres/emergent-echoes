using Godot;

public partial class Tree : StaticBody2D
{
	private Interactable _interactable;
	private Label _testLabel;
	
	private void OnInteract()
	{
		_testLabel.Text = "Interacted with tree!";
		_interactable.IsInteractable = false;
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_interactable = GetNode<Interactable>("Interactable"); 
		_testLabel = GetNode<Label>("TestLabel");

		_interactable.Interact = new Callable(this, nameof(OnInteract));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
