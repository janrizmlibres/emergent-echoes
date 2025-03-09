extends Node2D

func _ready():
	var total_food: ResourceStat = $TotalFood
	total_food.amount = 0
	total_food.weight = 1
	
	var actors = get_tree().get_nodes_in_group("Actors")
	WorldState.initialize(total_food, actors)

	var thresholds = ResourceStat.LOCAL_THRESHOLDS
	var food = Globals.ResourceType.FOOD
	var actor_count = WorldState.get_actor_count()

	total_food.lower_threshold = thresholds[food][0] * actor_count
	total_food.upper_threshold = thresholds[food][1] * actor_count
