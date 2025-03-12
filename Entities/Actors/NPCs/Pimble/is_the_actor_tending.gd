extends ConditionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") == "to tend":
		blackboard.set_value("current_state", "tending")
		return SUCCESS
	
	return FAILURE
