public interface InteractableObject
{
	bool IsInteractable { get; set; }
	string ActionDescription { get; set; }
	void Interact();  // Add this method to the interface
}
