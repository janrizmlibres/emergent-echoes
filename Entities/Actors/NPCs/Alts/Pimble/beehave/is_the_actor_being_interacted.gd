extends ConditionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") == "interacted":
		blackboard.set_value("current_state", "interacting")
		return SUCCESS
	
	return FAILURE
