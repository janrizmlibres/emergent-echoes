extends ActionLeaf

@onready var timer = $"../../../../Timer"

var timeout: bool

func before_run(actor: Node, blackboard: Blackboard) -> void:
	timer.start() 

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("is_idle") == false:
		return FAILURE
	
	if timeout:
		timeout = false
		return SUCCESS
		
	return RUNNING

func _on_timer_timeout() -> void:
	timeout = true
	pass # Replace with function body.
