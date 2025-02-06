extends ConditionLeaf

var actor_detected: bool
var actor

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_status") != "petitioning":
		return FAILURE

	if blackboard.get_value("actor_found") == true:
		return SUCCESS

	return FAILURE
