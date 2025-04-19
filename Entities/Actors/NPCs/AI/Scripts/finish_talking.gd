@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	var target = blackboard.get_value("data").target
	var increase_values := PCG.execute_talk(actor, target)

	actor.float_text_controller.show_float_text(
		PCG.ResourceType.COMPANIONSHIP,
		str(increase_values[0]),
		true
	)
	target.float_text_controller.show_float_text(
		PCG.ResourceType.COMPANIONSHIP,
		str(increase_values[1]),
		true
	)

	actor.executor.end_action()
	return SUCCESS
