@tool
@icon("../../icons/action.svg")
class_name ActionLeaf extends Leaf

signal on_patrol(patrol_location: Vector2)

@export var movement_speed: float = 50.0

var current_patrol_index: int = 0

const first_patrol_location = Vector2(312, 152)
const second_patrol_location = Vector2(200, 280)
const garreth_home_location = Vector2(296, 216)


func get_class_name() -> Array[StringName]:
	var classes := super()
	classes.push_back(&"ActionLeaf")
	return classes

func tick(actor:Node, blackboard:Blackboard) -> int:
	if current_patrol_index == 0:
		on_patrol.emit(first_patrol_location)
		current_patrol_index += 1
	elif current_patrol_index == 1:
		on_patrol.emit(second_patrol_location)
		current_patrol_index += 1
	elif current_patrol_index == 2:
		on_patrol.emit(garreth_home_location)
		current_patrol_index = 0
	return SUCCESS
