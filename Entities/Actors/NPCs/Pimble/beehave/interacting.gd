extends ActionLeaf

@onready var conversation_timer = $"../../../../ConversationTimer"
@onready var emote_bubble = $"../../../../EmoteBubble"
@onready var animation_tree = $"../../../../AnimationTree"
@onready var animation_state: AnimationNodeStateMachinePlayback = animation_tree.get("parameters/playback")
@onready var blackboard = $"../../../../Blackboard"

func before_run(actor: Node, blackboard: Blackboard) -> void:
	if blackboard.get_value("current_state") == "interacting":
		animation_state.travel("Idle")
		emote_bubble.activate()
		conversation_timer.start()
		return

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") == "interacting":
		actor.npc_active = false
		return RUNNING
		
	if blackboard.get_value("current_state") == "done interacting":
		conversation_timer.stop()
		actor.npc_active = true
		blackboard.set_value("current_state", "idle")
		return SUCCESS
	
	return FAILURE

func _on_conversation_timer_timeout() -> void:
	emote_bubble.deactivate()
	blackboard.set_value("current_state", "done interacting")
	pass # Replace with function body.
