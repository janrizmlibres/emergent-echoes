extends ActionLeaf

@onready var timer = $"../../../../Timer"
@onready var emote_controller = $"../../../../EmoteController"

var timeout: bool
var chances = 0

func before_run(actor: Node, blackboard: Blackboard) -> void:
	timer.start() 

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") != "patrolling":
		return FAILURE

	if chances >= 5:
		chances = 0
		return SUCCESS
		
	return RUNNING

func _on_timer_timeout() -> void:
	timer.start()
	emote_controller.ShowEmoteBubble(7)
	chances += 1
	pass # Replace with function body.
