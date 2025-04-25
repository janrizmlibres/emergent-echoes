extends ConditionLeaf

		
func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") == "surveying":
		if blackboard.get_value("actor") != null:
			if blackboard.get_value("actor").get_name() == "Player":
				blackboard.get_value("actor").stop_interaction()
		return SUCCESS
		
	if blackboard.get_value("current_state") == "idle":
		if blackboard.get_value("actor") != null:
			if blackboard.get_value("actor").get_name() == "Player":
				blackboard.get_value("actor").stop_interaction()
			
		blackboard.set_value("current_state", "shouting")
		return SUCCESS
	
	return FAILURE
