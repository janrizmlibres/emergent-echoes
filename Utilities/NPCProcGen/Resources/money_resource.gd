class_name MoneyResource
extends BaseResource

func _init(_amount: int, _weight: float):
	type = PCG.ResourceType.MONEY
	max_value = 1000
	is_tangible = true

	lower_threshold = 100
	upper_threshold = 900

	super._init(_amount, _weight)