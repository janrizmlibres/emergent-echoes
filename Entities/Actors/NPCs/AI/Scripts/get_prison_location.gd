@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	var prison_nodes = get_tree().get_nodes_in_group("Prisons")
	
	for prison in prison_nodes:
		var marker = prison as Prison
		if marker.current_capacity > 0:
			marker.current_capacity -= 1
			var move_position = get_omni_waypoint(actor.global_position, marker.global_position)
			blackboard.set_value("move_position", move_position)
			blackboard.set_value("prison_marker", marker)
	
	return SUCCESS

func get_omni_waypoint(origin: Vector2, move_position: Vector2):
	var direction = move_position.direction_to(origin)
	return move_position + direction * 12
