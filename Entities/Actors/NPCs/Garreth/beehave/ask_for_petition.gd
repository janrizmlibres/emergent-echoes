extends ActionLeaf

@onready var float_text_controller = $"../../../../FloatTextController"
@onready var emote_controller = $"../../../../EmoteController"
@onready var conversation_timer = $"../../../../ConversationTimer"
@onready var animation_tree =$"../../../../AnimationTree"
@onready var animation_state: AnimationNodeStateMachinePlayback = animation_tree.get("parameters/playback")

var chances = 0

func before_run(actor: Node, blackboard: Blackboard) -> void:
	if blackboard.get_value("current_state") != "petitioning":
		return
		
	if blackboard.get_value("actor").get_name() == "Player":
		blackboard.get_value("actor").OnInteractionStartedOnNPCAlt(blackboard.get_value("current_state"), actor)
		actor.npc_active = false
		return
	
	actor.npc_active = false
	conversation_timer.start()
	animation_state.travel("Idle")
	blackboard.get_value("actor").get_node("Blackboard").set_value("current_state", "interacted")
	return

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") == "petitioning":
		if blackboard.get_value("actor").get_name() == "Player":
			return RUNNING
		
		if chances >= 5:
			chances = 0
			conversation_timer.stop()
			blackboard.set_value("current_state", "petition answered")
		return RUNNING
		
	if blackboard.get_value("current_state") == "petition answered":
		conversation_timer.stop()
		emote_controller.ShowEmoteBubble(5)
		
		float_text_controller.ShowFloatText(0, "10", true)
		blackboard.set_value("money", blackboard.get_value("money") + 10)
		
		if blackboard.get_value("actor").get_name() == "Player":
			blackboard.get_value("actor").OnInteractionEnded()
		
		if blackboard.get_value("actor").get_name() != "Player":
			blackboard.get_value("actor").get_node("Blackboard").set_value("money", blackboard.get_value("actor").get_node("Blackboard").get_value("money") - 10)
		
		blackboard.set_value("current_state", "done petitioning")
		return SUCCESS
	
	return FAILURE

func _on_conversation_timer_timeout() -> void:
	emote_controller.ShowRandomEmote()
	chances += 1
	pass # Replace with function body.
