extends ActionLeaf

@onready var emote_bubble = $"../../../../EmoteBubble"

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("cutscene_state") == "hearing report from toblin":
		
		if blackboard.get_value("actor") != null && blackboard.get_value("actor").get_name() == "Player":
			blackboard.get_value("actor").stop_interaction()
		elif blackboard.get_value("actor") != null:
			blackboard.get_value("actor").get_node("Blackboard").set_value("current_state", "done interacting")
		
		blackboard.set_value("cutscene_state", "finished hearing report from toblin")
		
		actor.face_target(GameManager.toblin[0])
		actor.set_animation_to_idle()
		
		actor.npc_active = false
		emote_bubble.activate()
		return SUCCESS
	
	return FAILURE
