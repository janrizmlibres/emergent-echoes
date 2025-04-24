@tool
extends ActionLeaf

@export var threat_distance: float = 40

func tick(actor: Node, blackboard: Blackboard) -> int:
	var target: Actor = blackboard.get_value("data").target

	var move_position = blackboard.get_value("move_position")
	var distance = target.global_position.distance_to(actor.global_position)

	if move_position == null or distance < 40:
		var direction := target.global_position.direction_to(actor.global_position)
		var new_move_position = target.global_position + direction * threat_distance
		blackboard.set_value("move_position", new_move_position)
		return SUCCESS
	else:
		return FAILURE
