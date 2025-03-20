extends ActionLeaf

@onready var emote_bubble = $"../../../../EmoteBubble"

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("cutscene_state") == "inform garreth":
		GameManager.garreth[0].get_node("Blackboard").set_value("cutscene_state", "hearing information from toblin")
		
		actor.face_target(GameManager.garreth[0])
		actor.npc_active = false
		actor.set_animation_to_idle()
		emote_bubble.activate()

		blackboard.set_value("cutscene_state", "finished informing to garreth")
		return SUCCESS
	
	return FAILURE
