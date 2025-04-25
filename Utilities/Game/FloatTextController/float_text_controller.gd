class_name FloatTextController
extends Node2D

@export var duration: float = 2

var floating_text = preload("res://Entities/UI/Indicators/FloatingText/floating_text.tscn")

var spread: float = PI / 2
var travel: Vector2 = Vector2(0, -10)

func show_float_text(resource_type, value: String, is_normal: bool) -> void:
	var instance = floating_text.instantiate()

	var new_position = instance.global_position
	new_position.x -= instance.size.x / 2
	new_position.y -= 24
	instance.global_position = new_position

	add_child(instance)
	instance.show_value(resource_type, value, is_normal, duration, spread, travel)
