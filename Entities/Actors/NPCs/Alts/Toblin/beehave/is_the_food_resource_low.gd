extends ConditionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") != "idle":
		return FAILURE
	
	if blackboard.get_value("food_inventory") <= 4 && blackboard.get_value("money") >= 40:
		blackboard.set_value("current_state", "go to market")
		return SUCCESS
		
	return FAILURE
