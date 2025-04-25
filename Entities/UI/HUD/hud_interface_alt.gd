class_name HUDInterfaceAlt
extends Control

var player: Player

@onready var health_bar: TextureProgressBar = $VBoxContainer/HealthBar/TextureProgressBar
@onready var satiation_bar: TextureProgressBar = $VBoxContainer/HungerBar/TextureProgressBar
@onready var money_label: Label = $VBoxContainer2/MoneyLabel
@onready var food_label: Label = $VBoxContainer2/FoodLabel