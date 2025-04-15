class_name DutyResource
extends BaseResource

func _ready():
	type = PCG.ResourceType.DUTY
	max_value = 100
	is_tangible = false

	lower_threshold = 30
	upper_threshold = 90

func _physics_process(delta):
	var applied_decay = DECAY_RATE * delta
	amount += applied_decay if not WorldState.has_crimes() else -applied_decay