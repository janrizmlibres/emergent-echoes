class_name ActionCandidate
extends Node

var action_data: ActionData
var weight: float

func _init(_action_data: ActionData, _weight: float):
	action_data = _action_data
	weight = _weight
