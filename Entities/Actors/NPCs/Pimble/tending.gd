extends ActionLeaf

@onready var tending_timer = $"../../../../TendingTimer"
@onready var emote_controller = $"../../../../EmoteController"
@onready var animation_tree = $"../../../../AnimationTree"
@onready var animation_state: AnimationNodeStateMachinePlayback = animation_tree.get("parameters/playback")

func before_run(actor: Node, blackboard: Blackboard) -> void:
	if blackboard.get_value("current_state") == "tending":
		animation_state.travel("Idle")
		tending_timer.start()
		return

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") == "tending":
		actor.npc_active = false
		return RUNNING
		
	if blackboard.get_value("current_state") == "done tending":
		tending_timer.stop()
		actor.npc_active = true
		blackboard.set_value("current_state", "going back home")
		return SUCCESS
	
	return FAILURE

func _on_tending_timer_timeout() -> void:
	emote_controller.ShowRandomEmote()
	pass # Replace with function body.
