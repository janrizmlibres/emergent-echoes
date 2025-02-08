extends ActionLeaf

signal move_actor(set_state: String, patrol_location: Vector2)

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") != "going back home":
		return FAILURE
		
	if actor.navigation_agent_2d.is_navigation_finished():
		blackboard.set_value("current_state", "idle")
		
		return SUCCESS
	
	move_actor.emit(Vector2(720, 48))
	return RUNNING
