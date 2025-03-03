extends ActionLeaf

signal move_actor(set_state: String, patrol_location: Vector2)

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") != "go to market":
		return FAILURE
	
	move_actor.emit(Vector2(480, 496))
	
	if blackboard.get_value("agent_arrived") == true:
		blackboard.set_value("current_state", "buying food")
		blackboard.set_value("agent_arrived", false)
		return SUCCESS
		
	return RUNNING
