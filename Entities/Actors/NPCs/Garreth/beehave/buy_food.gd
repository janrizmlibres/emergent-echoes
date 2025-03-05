extends ActionLeaf

@onready var emote_controller = $"../../../../EmoteController"
@onready var float_text_controller = $"../../../../FloatTextController"

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") != "buying food":
		return FAILURE
		
	blackboard.set_value("food_inventory", blackboard.get_value("food_inventory") + 4)
	blackboard.set_value("money", blackboard.get_value("money") - 40)
	
	emote_controller.ShowEmoteBubble(5)
	float_text_controller.ShowFloatText(1, "4", false)
	
	blackboard.set_value("current_state", "resuming patrol")
	return SUCCESS
