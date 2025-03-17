extends Node

@onready var hunger_timer = $"../HungerTimer"
@onready var blackboard = $"../Blackboard"

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	hunger_timer.start()
	pass # Replace with function body.
	
func _process(delta: float) -> void:	
	if blackboard.get_value("is_full") <= 10 && blackboard.get_value("food_inventory") > 0:
		blackboard.set_value("is_full", blackboard.get_value("is_full") + 10)
		blackboard.set_value("food_inventory", blackboard.get_value("food_inventory") - 1)
		blackboard.set_value("current_state", "hungry")

func hunger_decay_timeout() -> void:
	blackboard.set_value("is_full", blackboard.get_value("is_full") - 10)
	pass # Replace with function body.
