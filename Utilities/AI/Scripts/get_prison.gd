@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	var prison := WorldState.get_available_prison()
	var move_position = get_omni_waypoint(actor.global_position, prison.global_position)
	
	blackboard.set_value("move_position", move_position)
	blackboard.set_value("prison", prison)
	return SUCCESS

func get_omni_waypoint(origin: Vector2, move_position: Vector2):
	var direction = move_position.direction_to(origin)
	return move_position + direction * 14
