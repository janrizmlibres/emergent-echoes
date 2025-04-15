class_name PlayerAgent
extends PCGAgent

@export_group("Resources")
@export var money_amount: int = 20
@export var food_amount: int = 2

func _ready():
	money_final = money_amount
	food_final = food_amount