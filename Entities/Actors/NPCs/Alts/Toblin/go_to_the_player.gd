extends ActionLeaf

signal move_actor(set_state: String, patrol_location: Vector2)

func tick(_actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("cutscene_state") != "go to the player":
		return FAILURE
	
	move_actor.emit(GameManager.player[0].global_position)
	
	if _actor.player_reached == true:
		_actor.current_location = Vector2.ZERO
		blackboard.set_value("cutscene_state", "talk to the player")
		blackboard.set_value("agent_arrived", false)
		
		_actor.player_reached = false
		return SUCCESS
			
	return RUNNING
