extends ActionLeaf 

@onready var float_text_controller = $"../../../../FloatTextController"
@onready var emote_controller = $"../../../../EmoteController"

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") != "stealing":
		return FAILURE
	
	blackboard.set_value("current_state", "going back home")
	return SUCCESS

func after_run(actor: Node, blackboard: Blackboard) -> void:
	float_text_controller.ShowFloatText(0, "100", false)
	emote_controller.ShowEmoteBubble(5)
	
	blackboard.get_value("actor").get_node_or_null("Blackboard").set_value("money", blackboard.get_value("actor").get_node_or_null("Blackboard").get_value("money") - 10)
	blackboard.set_value("money", blackboard.get_value("money") + 10)
