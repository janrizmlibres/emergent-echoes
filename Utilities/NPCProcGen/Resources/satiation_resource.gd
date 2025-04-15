class_name SatiationResource
extends BaseResource

func _ready():
	type = PCG.ResourceType.SATIATION
	max_value = 100
	is_tangible = false

	lower_threshold = 20
	upper_threshold = 90

func _physics_process(delta):
	amount -= DECAY_RATE * delta