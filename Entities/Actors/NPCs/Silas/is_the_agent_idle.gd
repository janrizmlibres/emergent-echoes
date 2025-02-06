extends ConditionLeaf


func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("is_idle") == true:
		return SUCCESS
		
	return FAILURE
