extends ActionLeaf

@onready var emote_bubble = $"../../../../../EmoteBubble"

func tick(actor: Node, blackboard: Blackboard) -> int:
	print(blackboard.get_value("cutscene_state"))
	if blackboard.get_value("cutscene_state") == "finished discussing with group":
		actor.npc_active = true
		emote_bubble.deactivate()
		blackboard.set_value("cutscene_state", "idle at the bonfire")
		return SUCCESS
	
	return FAILURE
