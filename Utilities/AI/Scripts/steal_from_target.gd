@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	var data: Dictionary = blackboard.get_value("data")

	var steal_amount := PCG.execute_theft(actor, data.target, data.resource_type)
	actor.float_text_controller.show_float_text(
		data.resource_type,
		str(steal_amount),
		false
	)
	return SUCCESS
