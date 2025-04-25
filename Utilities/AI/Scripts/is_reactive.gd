@tool
extends ConditionLeaf

func tick(_actor: Node, blackboard: Blackboard) -> int:
	return SUCCESS if blackboard.get_value("data").is_reactive else FAILURE
