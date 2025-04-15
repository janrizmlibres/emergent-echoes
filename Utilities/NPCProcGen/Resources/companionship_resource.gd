class_name CompanionshipResource
extends BaseResource

func _ready():
	type = PCG.ResourceType.COMPANIONSHIP
	max_value = 100
	is_tangible = false

	lower_threshold = 10
	upper_threshold = 90

func _physics_process(delta):
	amount -= DECAY_RATE * delta