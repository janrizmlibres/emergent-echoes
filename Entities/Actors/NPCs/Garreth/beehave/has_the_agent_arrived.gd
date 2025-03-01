extends ConditionLeaf


func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") == "surveying":
		return SUCCESS
		
	if blackboard.get_value("current_state") == "patrolling":
		if actor.navigation_agent_2d.is_navigation_finished():
			blackboard.set_value("current_state", "shouting")
			return SUCCESS
		else:
			return FAILURE
	else:
		return FAILURE
