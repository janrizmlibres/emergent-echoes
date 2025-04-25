extends ConditionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	if GameManager.crop_matured == true:
		blackboard.set_value("current_state", "extracting food")
		return SUCCESS
	
	return FAILURE
