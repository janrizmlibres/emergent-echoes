extends ActionLeaf

@onready var emote_bubble = $"../../../../EmoteBubble"

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("cutscene_state") == "be interrogated by garreth":
		blackboard.set_value("cutscene_state", "look to the left")
		
		actor.face_target(GameManager.garreth[0])
		actor.set_animation_to_idle()
		
		actor.npc_active = false
		emote_bubble.activate()
		return SUCCESS
	
	return FAILURE
