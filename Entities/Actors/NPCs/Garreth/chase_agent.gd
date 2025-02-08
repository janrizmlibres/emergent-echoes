extends ActionLeaf

signal move_player(patrol_location: Vector2)

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") != "petitioning":
		return FAILURE
		
	move_player.emit(blackboard.get_value("actor").get_position())
	return RUNNING 
