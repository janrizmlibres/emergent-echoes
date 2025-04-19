class_name PlayerAgent
extends PCGAgent

@export_group("Resources")
@export var money_amount := 20
@export var food_amount := 2

func _ready():
	money_final_amount = money_amount
	food_final_amount = food_amount