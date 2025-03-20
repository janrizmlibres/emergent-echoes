extends ActionLeaf

signal move_actor(set_state: String, patrol_location: Vector2)

@onready var tending_timer = get_tree().get_nodes_in_group("CropTimer")
@onready var crop_to_process = get_tree().get_nodes_in_group("Crops")

var current_crop_index = 0

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") != "extracting food":
		return FAILURE
		
	if current_crop_index < crop_to_process.size():
		move_actor.emit(crop_to_process[current_crop_index].global_position)
		if blackboard.get_value("agent_arrived") == true:
			actor.current_location = Vector2.ZERO
			crop_to_process[current_crop_index].frame = 0
			crop_to_process[current_crop_index].visible = false
			current_crop_index += 1
			blackboard.set_value("agent_arrived", false)
	else:
		current_crop_index = 0
		GameManager.crop_matured = false
		GameManager.are_there_crops = false
		blackboard.set_value("current_state", "going back home")
		return SUCCESS
	
	return RUNNING
