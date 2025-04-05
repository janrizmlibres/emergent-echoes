class_name CropTile
extends Node2D

enum Status {DORMANT, GROWING, MATURE}

@export var growth_duration: float = 60

var status: Status = Status.DORMANT:
	get:
		return status
	set(value):
		status = value
		if status == Status.DORMANT:
			growth_timer = 0
	
var is_attended: bool = false
var growth_timer: float = 0

@onready var sprite: Sprite2D = $Sprite2D

func _ready():
	visible = false

func _process(delta):
	if status != Status.GROWING:
		return

	growth_timer += delta

	if growth_timer >= growth_duration:
		status = Status.MATURE
		growth_timer = growth_duration
	
	var progress = get_growth_progress()
	if progress < 0.33:
		sprite.frame = 7
	elif progress < 0.66:
		sprite.frame = 8
	elif progress < 0.99:
		sprite.frame = 9
	else:
		sprite.frame = 10

func get_growth_progress() -> float:
	return growth_timer / growth_duration