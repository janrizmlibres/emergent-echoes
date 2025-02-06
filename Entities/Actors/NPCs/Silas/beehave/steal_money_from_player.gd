extends ActionLeaf 

@onready var float_text_controller = $"../../../../FloatTextController"
@onready var emote_controller = $"../../../../EmoteController"

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") != "stealing":
		return FAILURE
	
	blackboard.set_value("player_near", false)
	blackboard.set_value("player_stolen", true)
	return SUCCESS

func after_run(actor: Node, blackboard: Blackboard) -> void:
	float_text_controller.ShowFloatText("100")
	emote_controller.ShowEmoteBubble(5)
	
