extends ActionLeaf

@onready var emote_bubble = $"../../../../../EmoteBubble"

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("cutscene_state") == "finished discussing with group":
		actor.face_target(GameManager.toblin[0])
		actor.npc_active = true
		emote_bubble.deactivate()
		blackboard.set_value("cutscene_state", "idle at the bonfire")
		return SUCCESS
	
	return FAILURE
