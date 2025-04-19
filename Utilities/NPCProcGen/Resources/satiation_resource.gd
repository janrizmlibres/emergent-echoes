class_name SatiationResource
extends BaseResource

func _init(_amount: float, _weight: float):
	type = PCG.ResourceType.SATIATION
	max_value = 100
	is_tangible = false

	lower_threshold = 20
	upper_threshold = 90

	super._init(_amount, _weight)

func _physics_process(delta):
	amount -= DECAY_RATE * delta