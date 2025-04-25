extends ActionLeaf

@onready var timer = $"../../../../Timer"

var timeout = false

func before_run(actor: Node, blackboard: Blackboard) -> void:
	if blackboard.get_value("current_state") == "waiting":
		timer.start() 
		return

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") != "waiting":
		return FAILURE
	
	actor.set_animation_to_idle()
	
	if timeout:
		timeout = false
		blackboard.set_value("current_state", "patrolling")
		return SUCCESS
		
	return RUNNING

func _on_timer_timeout() -> void:
	timeout = true
	timer.stop()
	pass # Replace with function body.
