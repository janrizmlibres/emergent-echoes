extends ConditionLeaf

var navigation_finished = true	

func tick(_actor: Node, _blackboard: Blackboard) -> int:
	if navigation_finished:
		navigation_finished = false
		return SUCCESS
	return FAILURE

func patrol_finished() -> void:
	navigation_finished = true
	print("Silas is roaming to a new area")
	pass # Replace with function body.
