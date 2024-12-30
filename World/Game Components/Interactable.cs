using EmergentEchoes;
using Godot;

public partial class Interactable : Area2D
{
	[Export] public string ActionDescription = "";
	[Export] public bool IsInteractable = true;
	
	public Stats CharacterStats { get; set; }
	
	public Callable Interact;
}
