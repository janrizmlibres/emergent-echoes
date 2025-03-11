class_name ActorData

const DECAY_DURATION: float = 60.0
const PETITION_INTERVAL: float = 15.0

var relationship: float = 0.0:
	get:
		return relationship
	set(value):
		relationship = clamp(value, -35.0, 35.0)

var last_known_position: Vector2 = Vector2.INF:
	get:
		return last_known_position
	set(value):
		last_known_position = value
		decay_timer.start()

var decay_timer: Timer
var petition_timer: Timer

func _init(memorizer: Memorizer):
	decay_timer = Timer.new()
	decay_timer.wait_time = DECAY_DURATION
	decay_timer.one_shot = true
	decay_timer.timeout.connect(_on_decay_timer_timeout)
	memorizer.add_child(decay_timer)

func _on_decay_timer_timeout() -> void:
	last_known_position = Vector2.INF

static func get_interrogation_probability(relationship_level: float) -> float:
	if relationship_level <= -26:
		return 0.30
	elif relationship_level <= -16:
		return 0.50
	elif relationship_level <= -6:
		return 0.70
	elif relationship_level <= 4:
		return 0.90
	elif relationship_level <= 14:
		return 0.92
	elif relationship_level <= 24:
		return 0.95
	else:
		return 1.00
