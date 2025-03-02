extends Node

@onready var physciological_timer = $"../PhysciologicalTimer"
@onready var blackboard = $"../Blackboard"

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	physciological_timer.start()
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_physciological_timer_timeout() -> void:
	blackboard.set_value("is_full", blackboard.get_value("if_full") - 10)
	physciological_timer.start()
	pass # Replace with function body.
