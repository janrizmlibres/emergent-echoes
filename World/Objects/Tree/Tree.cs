using System.Threading.Tasks;
using Godot;

public partial class Tree : StaticBody2D
{
	[Export] public int TreeHealth = 100;
	
	private Interactable _interactable;
	private Label _testLabel;
	
	private async void OnInteract()
	{
		_interactable.IsInteractable = false;
		_testLabel.Text = "Tree Chopped";
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_interactable = GetNode<Interactable>("Interactable"); 
		_testLabel = GetNode<Label>("TestLabel");

		_interactable.Interact = new Callable(this, nameof(OnInteract));
	}
}
