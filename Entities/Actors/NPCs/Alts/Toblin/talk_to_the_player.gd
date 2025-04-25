extends ActionLeaf

@onready var emote_bubble = $"../../../../EmoteBubble"

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("cutscene_state") == "talk to the player":
		GameManager.player[0].start_interaction(actor)
		
		actor.face_target(GameManager.player[0])
		actor.npc_active = false
		actor.set_animation_to_idle()
		emote_bubble.activate()

		blackboard.set_value("cutscene_state", "finished talking to player")
		return SUCCESS
	
	return FAILURE
