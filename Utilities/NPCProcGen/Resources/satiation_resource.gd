class_name SatiationResource
extends BaseResource

var actor: Actor

func _init(_actor: Actor, _amount: float, _weight: float):
	actor = _actor

	type = PCG.ResourceType.SATIATION
	max_value = 100
	is_tangible = false

	lower_threshold = 20
	upper_threshold = 90

	super._init(_amount, _weight)

func _physics_process(delta):
	amount -= DECAY_RATE * delta
	if amount <= 0:
		PCG.emit_satiation_depleted(actor)