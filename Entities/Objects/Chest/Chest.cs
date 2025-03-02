using Godot;
using NPCProcGen;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Components.Enums;

public partial class Chest : StaticBody2D
{
	[Export] public int Gold = 100;
	[Export] public int GoldAmount = 10;
	[Export] public string ChestOwner = "Garreth";

	private Interactable _interactable;
	private ActorTag2D _actorTag;
	private Label _label;
	private InteractingComponent _interactingComponent;

	private void OnDeductGold()
	{
		if (_actorTag == null) return;
		if (Gold <= 0) return;

		ResourceManager.Instance.ModifyResource(ResourceType.Money, GoldAmount, _actorTag);
		Gold -= GoldAmount;
		_label.Text = "Gold: " + Gold;
	}

	private void OnAddGold()
	{
		if (_actorTag == null) return;

		if (ResourceManager.Instance.GetResourceAmount(ResourceType.Money, _actorTag) < GoldAmount)
			return;

		ResourceManager.Instance.ModifyResource(ResourceType.Money, -GoldAmount, _actorTag);
		Gold += GoldAmount;
		_label.Text = "Gold: " + Gold;
	}

	private void _on_chest_area_area_entered(Area2D area)
	{
		if (area.Name != "InteractRange") return;

		_interactingComponent = area.GetParent<InteractingComponent>();

		_label.Text = "Gold: " + Gold;
		_label.Show();
		_actorTag = _interactingComponent.GetParent().GetNode<ActorTag2D>("ActorTag2D");
	}

	private void _on_chest_area_area_exited(Area2D area)
	{
		if (area.Name != "InteractRange") return;

		_interactingComponent = area.GetParent<InteractingComponent>();

		_label.Hide();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_interactable = GetNode<Interactable>("Interactable");
		_label = GetNode<Label>("Label");

		_interactable.PrimaryInteract = new Callable(this, nameof(OnDeductGold));
		_interactable.SecondaryInteract = new Callable(this, nameof(OnAddGold));
	}
}
