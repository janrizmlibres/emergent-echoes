extends ConditionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("is_idle") == true:
		print("Silas is roaming the lands")
		blackboard.set_value("is_idle", false)
		return SUCCESS
		
	return FAILURE
