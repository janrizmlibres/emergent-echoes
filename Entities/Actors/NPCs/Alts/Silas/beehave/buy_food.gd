extends ActionLeaf

@onready var emote_controller = $"../../../../EmoteController"
@onready var float_text_controller = $"../../../../FloatTextController"

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") != "buying food":
		return FAILURE
		
	blackboard.set_value("food_inventory", blackboard.get_value("food_inventory") + 2)
	blackboard.set_value("money", blackboard.get_value("money") - 20)
	
	float_text_controller.ShowFloatText(Globals.ResourceType.TOTAL_FOOD, "2", true)
	
	blackboard.set_value("current_state", "idle")
	return SUCCESS
