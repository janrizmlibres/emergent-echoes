class_name FoodResource
extends BaseResource

func _ready():
	type = PCG.ResourceType.FOOD
	max_value = 50
	is_tangible = true

	lower_threshold = PCG.food_lower_threshold
	upper_threshold = PCG.food_upper_threshold