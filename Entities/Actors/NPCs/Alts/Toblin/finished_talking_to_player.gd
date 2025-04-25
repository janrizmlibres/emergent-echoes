extends ActionLeaf

@onready var emote_bubble = $"../../../../../EmoteBubble"

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("cutscene_state") == "finished talking to player":
		GameManager.player[0].stop_interaction()
		actor.npc_active = true
		emote_bubble.deactivate()
		blackboard.set_value("cutscene_state", "go to garreth to inform")
		return SUCCESS
	
	return FAILURE
