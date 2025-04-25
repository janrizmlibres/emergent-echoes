extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("cutscene_state") == "idle at the bonfire":
		return RUNNING
		
	if blackboard.get_value("cutscene_state") == "hearing information from toblin":
		return SUCCESS
	
	return FAILURE
