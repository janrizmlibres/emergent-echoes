extends Node2D

@onready var total_food: ResourceStat = $TotalFood
@onready var label: Label = $HUD/TotalFoodLabel

func _ready():
	var shop: Shop = $Shop
	total_food.amount = shop.food_amount
	total_food.weight = 1
	
	var actors = get_tree().get_nodes_in_group("Actors")
	WorldState.initialize(total_food, shop, actors)

	var thresholds = ResourceStat.LOCAL_THRESHOLDS
	var food = Globals.ResourceType.FOOD
	var actor_count = WorldState.get_actor_count()

	total_food.lower_threshold = thresholds[food][0] * actor_count
	total_food.upper_threshold = thresholds[food][1] * actor_count

	label.text = "Total Food: " + str(total_food.amount)

func _on_total_food_total_food_updated(amount: int):
	label.text = "Total Food: " + str(amount)
