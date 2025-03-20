extends ActionLeaf

func tick(_actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("cutscene_state") != "death by being hungry":
		return FAILURE
	
	GameManager.show_perished_message("Toblin")
	_actor.queue_free()
			
	return SUCCESS
