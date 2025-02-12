using EmergentEchoes;
using Godot;

public partial class Interactable : Area2D
{
	[Export] public string PrimaryActionDescription = "";
	[Export] public string SecondaryActionDescription = "";
	[Export] public bool HasSecondaryAction = false;
	[Export] public bool IsInteractable = true;
	
	public Callable PrimaryInteract;
	public Callable SecondaryInteract;
}
