@tool
extends ActionLeaf

func tick(actor: Node, _blackboard: Blackboard) -> int:
	var quantity := PCG.execute_shop(actor)
	actor.float_text_controller.show_float_text(
		PCG.ResourceType.FOOD,
		str(quantity),
		true
	)
	return SUCCESS