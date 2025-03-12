extends Node2D

@onready var crops = $Crops
@onready var delay_timer = $DelayTimer
@onready var growth_timer = $GrowthTimer

var growing = false
var pimble_entered = false
# Array to hold the child nodes of Crops to process
var children_to_process = []
# Index to track which child node to update next
var current_index = 0
var pimble_actor: CharacterBody2D

func _on_area_2d_body_entered(body: Node2D) -> void:
	print(body.get_name())
	# Check if the body is "PimbleAlt" and the process hasn’t started yet
	if body.get_name() == "pimbleAlt" and not pimble_entered and not growing:
		pimble_actor = body
		pimble_actor.get_node("Blackboard").set_value("current_state", "to tend")
		pimble_entered = true
		# Get all child nodes of Crops (assumed to be Sprite2D nodes)
		children_to_process = crops.get_children()
		current_index = 0
		# Start the timer to begin the delayed frame updates
		
		delay_timer.start()

func _on_delay_timer_timeout() -> void:
	# Check if there are still children to process
	if current_index < children_to_process.size():
		# Set the frame of the current Sprite2D child to 1
		children_to_process[current_index].frame = 1
		# Move to the next child
		current_index += 1
	else:
		growing = true
		pimble_entered = false
		pimble_actor.get_node("Blackboard").set_value("current_state", "done tending")
		delay_timer.stop()
