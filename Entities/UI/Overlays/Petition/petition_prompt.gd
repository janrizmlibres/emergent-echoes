class_name PetitionPrompt
extends CanvasLayer

var petitioner: Actor
var resource_type: PCG.ResourceType
var quantity: int

@onready var confirm_label: ConfirmationLabel = $Prompt/VBoxContainer/ConfirmationLabel

func initialize(_petitioner: Actor, _resource_type: PCG.ResourceType, _quantity: int):
	petitioner = _petitioner
	resource_type = _resource_type
	quantity = _quantity

	var resource_name := PCG.resource_to_string(resource_type)
	confirm_label.UpdateText(str(quantity), resource_name, petitioner.name)

func _on_yes_pressed():
	var player := WorldState.get_player()
	WorldState.resource_manager.transfer_resource(player, petitioner, resource_type, quantity)
	WorldState.memory_manager.modify_relationship(petitioner, player, 10)

	petitioner.float_text_controller.show_float_text(
		resource_type,
		str(quantity),
		true
	)

	petitioner.set_main_state(NPC.MainState.WANDER)
	unpause()

func _on_no_pressed():
	petitioner.float_text_controller.show_float_text(
		PCG.ResourceType.COMPANIONSHIP,
		"-1",
		true
	)
	petitioner.set_main_state(NPC.MainState.WANDER)
	unpause()

func unpause() -> void:
	queue_free()
	get_tree().paused = false