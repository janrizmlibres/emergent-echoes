class_name TotalFoodResource
extends BaseResource

func _init():
	type = PCG.ResourceType.TOTAL_FOOD
	max_value = Globals.INT_MAX
	is_tangible = true
	weight = 1

	lower_threshold = 0
	upper_threshold = 0

func set_thresholds(lower: int, upper: int):
	lower_threshold = lower
	upper_threshold = upper