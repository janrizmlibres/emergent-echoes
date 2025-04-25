extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("cutscene_state") == "idle at the farm":
		return RUNNING
		
	if blackboard.get_value("cutscene_state") == "go to bonfire":
		return SUCCESS
	
	return FAILURE
