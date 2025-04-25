extends ConditionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("cutscene_state") == "farmer is dead":
		GameManager.set_total_food("3")
		blackboard.set_value("cutscene_state", "inspecting the farmer")
		return SUCCESS
	
	return FAILURE
