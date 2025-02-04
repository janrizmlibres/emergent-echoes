@tool
@icon("../../icons/condition.svg")
class_name ConditionLeaf extends Leaf

var navigation_finished = true

## Conditions are leaf nodes that either return SUCCESS or FAILURE depending on
## a single simple condition. They should never return `RUNNING`.

func get_class_name() -> Array[StringName]:
	var classes := super()
	classes.push_back(&"ConditionLeaf")
	return classes

func tick(actor:Node, blackboard:Blackboard) -> int:
	if navigation_finished:
		navigation_finished = false
		return SUCCESS
	return FAILURE


func _on_navigation_agent_2d_navigation_finished() -> void:
	navigation_finished = true
	pass # Replace with function body.
