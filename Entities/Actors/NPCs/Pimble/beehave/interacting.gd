extends ActionLeaf

@onready var conversation_timer = $"../../../../ConversationTimer"
@onready var emote_controller = $"../../../../EmoteController"
@onready var animation_tree = $"../../../../AnimationTree"
@onready var animation_state: AnimationNodeStateMachinePlayback = animation_tree.get("parameters/playback")

var chances = 0

func before_run(actor: Node, blackboard: Blackboard) -> void:
	if blackboard.get_value("current_state") == "interacting":
		animation_state.travel("Idle")
		conversation_timer.start()
		return

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") == "interacting":
		actor.npc_active = false
		
		if chances >= 5:
			chances = 0
			conversation_timer.stop()
			blackboard.set_value("current_state", "done interacting")
			
		return RUNNING
		
	if blackboard.get_value("current_state") == "done interacting":
		conversation_timer.stop()
		actor.npc_active = true
		blackboard.set_value("current_state", "idle")
		return SUCCESS
	
	return FAILURE

func _on_conversation_timer_timeout() -> void:
	emote_controller.ShowRandomEmote()
	chances += 1
	pass # Replace with function body.
