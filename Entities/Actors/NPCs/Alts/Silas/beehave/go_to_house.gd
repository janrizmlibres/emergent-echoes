extends ActionLeaf

signal move_actor(set_state: String, patrol_location: Vector2)

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") != "going back home":
		return FAILURE
		
	if blackboard.get_value("agent_arrived") == true:
		actor.current_location = Vector2.ZERO
		blackboard.set_value("agent_arrived", false)
		blackboard.set_value("current_state", "idle")
		return SUCCESS
	
	move_actor.emit(Vector2(752, 192))
	return RUNNING
