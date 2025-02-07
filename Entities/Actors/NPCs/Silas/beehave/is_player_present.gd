extends ConditionLeaf

@onready var emote_controller = $"../../../../EmoteController"

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("actor_found") == true:
		blackboard.set_value("current_state", "chasing")
		
		return SUCCESS
	
	return FAILURE
