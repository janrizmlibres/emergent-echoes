class_name PlayerAgent
extends PCGAgent

@export_group("Resources")
@export var money_amount := 20
@export var food_amount := 2
@export var satiation_amount := 100.0

func _ready():
	money_final_amount = money_amount
	food_final_amount = food_amount
	satiation_final_amount = satiation_amount