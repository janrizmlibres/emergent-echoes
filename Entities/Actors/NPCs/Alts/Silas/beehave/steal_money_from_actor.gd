extends ActionLeaf 

@onready var float_text_controller = $"../../../../FloatTextController"

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") != "stealing":
		return FAILURE
	
	blackboard.set_value("current_state", "going back home")
	return SUCCESS

func after_run(actor: Node, blackboard: Blackboard) -> void:
	float_text_controller.show_float_text(Globals.ResourceType.MONEY, "10", false)
	blackboard.set_value("money", blackboard.get_value("money") + 10)
	
	if blackboard.get_value("actor").get_name() != "Player":
		blackboard.get_value("actor").get_node_or_null("Blackboard").set_value("money", blackboard.get_value("actor").get_node_or_null("Blackboard").get_value("money") - 10)
	
