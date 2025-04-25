extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") != "buying food":
		return FAILURE
		
	blackboard.set_value("food_inventory", blackboard.get_value("food_inventory") + 4)
	blackboard.set_value("money", blackboard.get_value("money") - 40)
	
	actor.float_text_controller.show_float_text(PCG.ResourceType.FOOD, "4", true)
	
	blackboard.set_value("current_state", "resuming patrol")
	return SUCCESS
