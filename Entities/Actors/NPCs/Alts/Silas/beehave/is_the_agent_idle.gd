extends ConditionLeaf


func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") == "idle":
		blackboard.set_value("current_state", "waiting")
		return SUCCESS
	
	return FAILURE
