extends ConditionLeaf


func tick(actor: Node, blackboard: Blackboard) -> int:
	if actor.navigation_agent_2d.is_navigation_finished():
		return SUCCESS
	return FAILURE
