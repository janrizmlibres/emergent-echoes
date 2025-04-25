extends ActionLeaf

@onready var emote_bubble = $"../../../../../EmoteBubble"

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("cutscene_state") == "finished reporting to garreth":
		actor.npc_active = true
		emote_bubble.deactivate()
		blackboard.set_value("cutscene_state", "go and stay at the farm")
		return SUCCESS
	
	return FAILURE
