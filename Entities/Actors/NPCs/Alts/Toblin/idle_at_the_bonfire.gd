extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("cutscene_state") == "idle at the Bonfire":
		return RUNNING
		
	if blackboard.get_value("cutscene_state") == "go to market":
		return SUCCESS
	
	return FAILURE
