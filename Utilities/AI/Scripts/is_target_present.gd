@tool
extends ConditionLeaf

func tick(_actor: Node, blackboard: Blackboard) -> int:
	var target = blackboard.get_value("data").get("target")

	if target == null:
		return FAILURE

	return SUCCESS if WorldState.has_actor(target) else FAILURE