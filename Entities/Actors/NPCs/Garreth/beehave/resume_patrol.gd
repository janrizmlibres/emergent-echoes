extends ActionLeaf

signal move_actor(patrol_location: Vector2)

func before_run(actor: Node, blackboard: Blackboard) -> void:
	if blackboard.get_value("current_state") != "resuming patrol":
		return
		
	if blackboard.get_value("actor").get_name() == "Player":
		blackboard.get_value("actor").OnInteractionEnded()

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") == "resuming patrol" || blackboard.get_value("current_state") == "failed petition":
		move_actor.emit(blackboard.get_value("last_patrol_location"))
	
		if actor.navigation_agent_2d.is_navigation_finished():
			if blackboard.get_value("current_state") == "failed petition":
				blackboard.set_value("current_state", "surveying")
				return SUCCESS
				
			blackboard.set_value("current_state", "idle")
			return SUCCESS
			
		return RUNNING
	else:
		return FAILURE
