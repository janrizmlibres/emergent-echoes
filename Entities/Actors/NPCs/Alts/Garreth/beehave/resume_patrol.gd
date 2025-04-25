extends ActionLeaf

signal move_actor(patrol_location: Vector2)

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") == "resuming patrol" || blackboard.get_value("current_state") == "done petitioning":
		move_actor.emit(blackboard.get_value("last_patrol_location"))
	
		if blackboard.get_value("agent_arrived") == true:
			if blackboard.get_value("current_state") == "done petitioning":
				actor.current_location = Vector2.ZERO
				blackboard.set_value("agent_arrived", false)
				blackboard.set_value("current_state", "surveying")
				return SUCCESS
				
			actor.current_location = Vector2.ZERO
			blackboard.set_value("agent_arrived", false)
			blackboard.set_value("current_state", "idle")
			return SUCCESS
			
		return RUNNING
	else:
		return FAILURE
