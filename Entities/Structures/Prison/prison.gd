class_name Prison
extends Marker2D

@export var max_capacity: int = 1

var current_capacity: int = max_capacity:
  get:
    return current_capacity
  set(value):
    current_capacity = clamp(value, 0, max_capacity)