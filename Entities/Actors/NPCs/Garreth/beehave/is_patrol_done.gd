extends ConditionLeaf

var navigation_finished = true	

func tick(_actor: Node, _blackboard: Blackboard) -> int:
	if navigation_finished:
		navigation_finished = false
		return SUCCESS
	return FAILURE

func _on_navigation_agent_2d_navigation_finished() -> void:
	navigation_finished = true
	pass # Replace with function body.
