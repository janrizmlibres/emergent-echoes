extends ActionLeaf

@onready var emote_bubble = $"../../../../EmoteBubble"

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("cutscene_state") == "hearing information from toblin":
		blackboard.set_value("cutscene_state", "finished hearing information from toblin")
		
		actor.face_target(GameManager.toblin[0])
		actor.set_animation_to_idle()
		
		actor.npc_active = false
		emote_bubble.activate()
		return SUCCESS
	
	return FAILURE
