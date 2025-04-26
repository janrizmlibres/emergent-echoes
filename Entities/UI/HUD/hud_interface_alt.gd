class_name HUDInterfaceAlt
extends Control

var event_label: PackedScene = preload("res://Entities/UI/HUD/event_label.tscn")

var player: PlayerAlt = null

@onready var health_bar: TextureProgressBar = $VBoxContainer/HealthBar/TextureProgressBar
@onready var satiation_bar: TextureProgressBar = $VBoxContainer/HungerBar/TextureProgressBar
@onready var money_label: Label = $VBoxContainer2/MoneyLabel
@onready var food_label: Label = $VBoxContainer2/FoodLabel
@onready var event_container: VBoxContainer = $VBoxContainer3

func _ready():
	player = get_tree().get_first_node_in_group("Player")

func _process(_delta):
	if player == null:
		return
		
	health_bar.value = player.hit_points
	satiation_bar.value = player.satiation
	money_label.text = "Money: " + str(player.money)
	food_label.text = "Food: " + str(player.food)

func broadcast_event(event: String):
	var event_label_instance = event_label.instantiate()
	event_label_instance.text = event
	event_container.add_child(event_label_instance)