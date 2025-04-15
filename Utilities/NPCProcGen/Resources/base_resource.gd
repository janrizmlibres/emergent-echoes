class_name BaseResource
extends Node

const DECAY_RATE := 0.2

var max_value: int
var is_tangible: bool

var lower_threshold: int
var upper_threshold: int

var type: PCG.ResourceType
var weight: float

var amount := 0.0:
	get:
		return amount
	set(value):
		amount = clamp(value, 0, max_value)
		amount = floor(amount) if is_tangible else amount

func _init(_amount := 0.0):
	amount = amount

func is_unbounded() -> bool:
	return max_value == Globals.INT_MAX

func get_deficiency_point() -> float:
	return lerp(lower_threshold, upper_threshold - 1, weight)