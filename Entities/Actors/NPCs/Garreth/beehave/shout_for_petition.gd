extends ActionLeaf

@onready var timer = $"../../../../Timer"
@onready var emote_controller = $"../../../../EmoteController"

var chances = 0

func before_run(actor: Node, blackboard: Blackboard) -> void:
	if blackboard.get_value("current_state") == "shouting" || blackboard.get_value("current_state") == "surveying":
		timer.start()
		return

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") != "shouting" && blackboard.get_value("current_state") != "surveying":
		return FAILURE
			
	if chances >= 5:
		chances = 0
		timer.stop()
		blackboard.set_value("current_state", "patrolling")
		return SUCCESS
			
	return RUNNING

func _on_timer_timeout() -> void:
	timer.start()
	emote_controller.ShowEmoteBubble(7)
	chances += 1
	pass # Replace with function body.
