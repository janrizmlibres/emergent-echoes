class_name HUDInterface
extends Control

var player: Player
var satiation_resource: SatiationResource
var money_resource: MoneyResource
var food_resource: FoodResource

@onready var health_bar: TextureProgressBar = $VBoxContainer/HealthBar/TextureProgressBar
@onready var satiation_bar: TextureProgressBar = $VBoxContainer/HungerBar/TextureProgressBar
@onready var money_label: Label = $VBoxContainer2/MoneyLabel
@onready var food_label: Label = $VBoxContainer2/FoodLabel

func initialize():
	var resource_mgr = WorldState.resource_manager
	player = WorldState.get_player()

	satiation_resource = resource_mgr.get_resource(player, PCG.ResourceType.SATIATION)
	money_resource = resource_mgr.get_resource(player, PCG.ResourceType.MONEY)
	food_resource = resource_mgr.get_resource(player, PCG.ResourceType.FOOD)

	money_label.text = "Money: " + str(money_resource.amount as int)
	food_label.text = "Food: " + str(food_resource.amount as int)

func _process(_delta):
	health_bar.value = player.hit_points
	satiation_bar.value = satiation_resource.amount
	money_label.text = "Money: " + str(money_resource.amount)
	food_label.text = "Food: " + str(food_resource.amount)
