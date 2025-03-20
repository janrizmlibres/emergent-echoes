extends ActionLeaf

signal move_actor(set_state: String, patrol_location: Vector2)

func tick(_actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("cutscene_state") != "go to silas":
		return FAILURE
	
	GameManager.silas[0].get_node("Blackboard").set_value("cutscene_state", "garreth is looking for you")
	move_actor.emit(GameManager.silas[0].global_position)
	
	if _actor.silas_reached == true:
		_actor.current_location = Vector2.ZERO
		blackboard.set_value("cutscene_state", "interrogate silas")
		blackboard.set_value("agent_arrived", false)
		
		_actor.silas_reached = false
		return SUCCESS
			
	return RUNNING
