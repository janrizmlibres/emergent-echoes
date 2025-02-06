extends ConditionLeaf

@onready var emote_controller = $"../../../../EmoteController"

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("player_found") == true:
		blackboard.set_value("current_state", "stealing")
		
		emote_controller.ShowEmoteBubble(4)
		return SUCCESS
	
	return FAILURE
