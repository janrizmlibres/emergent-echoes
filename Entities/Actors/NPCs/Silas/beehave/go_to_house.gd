extends ActionLeaf

signal move_player(set_state: String, patrol_location: Vector2)

func tick(actor: Node, blackboard: Blackboard) -> int:
	if actor.navigation_agent_2d.is_navigation_finished():
		blackboard.set_value("is_idle", true)
		blackboard.set_value("player_found", false)
		blackboard.set_value("player_near", false)
		return SUCCESS
	
	move_player.emit(Vector2(720, 48))
	return RUNNING
