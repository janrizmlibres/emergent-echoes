extends ActionLeaf

@onready var emote_bubble = $"../../../../EmoteBubble"

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("cutscene_state") == "interrogate silas":
		GameManager.silas[0].get_node("Blackboard").set_value("current_state", "interrupted")
		GameManager.silas[0].get_node("Blackboard").set_value("cutscene_state", "be interrogated by garreth")
		
		GameManager.set_total_food("2")
		
		actor.face_target(GameManager.silas[0])
		actor.set_animation_to_idle()
		
		actor.npc_active = false
		emote_bubble.activate()
		
		blackboard.set_value("cutscene_state", "finished interrogating silas")
		return SUCCESS
	
	return FAILURE
