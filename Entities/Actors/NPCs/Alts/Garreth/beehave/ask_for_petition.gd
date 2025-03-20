extends ActionLeaf

@onready var float_text_controller = $"../../../../FloatTextController"
@onready var emote_bubble = $"../../../../EmoteBubble"
@onready var conversation_timer = $"../../../../ConversationTimer"
@onready var blackboard = $"../../../../Blackboard"

func before_run(actor: Node, blackboard: Blackboard) -> void:
	if blackboard.get_value("current_state") != "petitioning":
		return
		
	if blackboard.get_value("actor").get_name() == "Player":
		blackboard.get_value("actor").start_interaction(actor)
	else:
		blackboard.get_value("actor").face_target(actor)
		blackboard.get_value("actor").get_node("Blackboard").set_value("current_state", "interacted")
	
	actor.face_target(blackboard.get_value("actor"))
	actor.npc_active = false
	emote_bubble.activate()
	conversation_timer.start()
	actor.set_animation_to_idle()
	return

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") == "petitioning":
		return RUNNING
		
	if blackboard.get_value("current_state") == "petition answered":
		conversation_timer.stop()
				
		float_text_controller.show_float_text(Globals.ResourceType.MONEY, "10", true)
		blackboard.set_value("money", blackboard.get_value("money") + 10)
		
		if blackboard.get_value("actor").get_name() == "Player":
			blackboard.get_value("actor").stop_interaction()
		else:
			blackboard.get_value("actor").get_node("Blackboard").set_value("money", blackboard.get_value("actor").get_node("Blackboard").get_value("money") - 10)
		
		blackboard.set_value("current_state", "done petitioning")
		return SUCCESS
	
	return FAILURE

func _on_conversation_timer_timeout() -> void:
	emote_bubble.deactivate()
	blackboard.set_value("current_state", "petition answered")
	pass # Replace with function body.
