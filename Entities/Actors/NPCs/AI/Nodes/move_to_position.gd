@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	var move_position: Vector2 = blackboard.get_value("move_position")
	actor.navigation_agent.target_position = move_position

	if actor.navigation_agent.is_navigation_finished():
		return SUCCESS
	
	actor.move_agent()
	return RUNNING
