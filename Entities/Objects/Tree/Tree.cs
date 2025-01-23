using System.Threading.Tasks;
using Godot;

public partial class Tree : StaticBody2D
{
	private Interactable _interactable;
	private Sprite2D _saplingSprite;
	private Sprite2D _treeSprite;
	private Timer _regrowTimer;
	
	private void OnInteract()
	{
		_interactable.IsInteractable = false;
		_saplingSprite.Show();
		_treeSprite.Hide();
		_regrowTimer.Start();
	}
	
	private void OnRegrowTimerTimeout()
	{
		GD.Print("Tree Grown");
		_interactable.IsInteractable = true;
		_saplingSprite.Hide();
		_treeSprite.Show();
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_interactable = GetNode<Interactable>("Interactable"); 
		_saplingSprite = GetNode<Sprite2D>("SaplingSprite");
		_treeSprite = GetNode<Sprite2D>("TreeSprite");
		_regrowTimer = GetNode<Timer>("RegrowTimer");

		_interactable.Interact = new Callable(this, nameof(OnInteract));
	}
}
