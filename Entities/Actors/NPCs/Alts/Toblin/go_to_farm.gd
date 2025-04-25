extends ActionLeaf

signal move_actor(set_state: String, patrol_location: Vector2)

func tick(_actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("cutscene_state") != "go and stay at the farm":
		return FAILURE
	
	move_actor.emit(Vector2(776, 448))
	
	if blackboard.get_value("agent_arrived") == true:
		_actor.current_location = Vector2.ZERO
		_actor.set_animation_to_idle()
		blackboard.set_value("cutscene_state", "idle at the farm")
		blackboard.set_value("agent_arrived", false)
		return SUCCESS
			
	return RUNNING
