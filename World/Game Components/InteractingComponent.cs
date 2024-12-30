using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmergentEchoes;

public partial class InteractingComponent : Node2D
{
	private Label _interactLabel;
	
	private readonly List<Interactable> _currentInteractions = new List<Interactable>();
	private bool _canInteract = true;

	private void OnInteractRangeAreaEntered(Interactable area)
	{
		_currentInteractions.Add(area);
	}
	
	private void OnInteractRangeAreaExited(Interactable area)
	{
		_currentInteractions.Remove(area);
	}

	private int SortByNearest(Interactable area1, Interactable area2)
	{
		var area1Dist = GlobalPosition.DistanceSquaredTo(area1.GlobalPosition);
		var area2Dist = GlobalPosition.DistanceSquaredTo(area2.GlobalPosition);
		
		return area1Dist.CompareTo(area2Dist);
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_interactLabel = GetNode<Label>("InteractLabel");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_currentInteractions.Count > 0 && _canInteract)
		{
			_currentInteractions.Sort(SortByNearest);
			
			if (!_currentInteractions[0].IsInteractable) { return; }
			_interactLabel.Text = "[E] to " + _currentInteractions[0].ActionDescription;
			_interactLabel.Show();
		}
		else
		{
			_interactLabel.Hide();
		}
	}
	
	public override async void _Input(InputEvent @event)
	{
		if (!@event.IsActionPressed("interact") || !_canInteract)
		{
			return;
		}

		if (_currentInteractions.Count == 0)
		{
			return;
		}
	
		_canInteract = false;
		_interactLabel.Hide();
		
		_currentInteractions[0].CharacterStats = GetParent().GetNode<Stats>("Stats");
		await Task.FromResult(_currentInteractions[0].Interact.Call());

		_canInteract = true;
	}
}
