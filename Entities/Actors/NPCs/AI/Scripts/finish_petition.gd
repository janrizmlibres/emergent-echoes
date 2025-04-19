@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	var target: Actor = blackboard.get_value("data").target
	var resource_type: PCG.ResourceType = blackboard.get_value("data").resource_type
	
	var result := PCG.execute_petition(actor, target, resource_type)

	if result[0]:
		actor.float_text_controller.show_float_text(
			resource_type,
			str(result[1]),
			true
		)
	else:
		actor.float_text_controller.show_float_text(
			PCG.ResourceType.COMPANIONSHIP,
			str(PCG.PETITION_DECREASE),
			true
		)

	actor.executor.end_action()
	return SUCCESS
