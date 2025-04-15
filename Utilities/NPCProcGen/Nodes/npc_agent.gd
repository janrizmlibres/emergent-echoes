class_name NPCAgent
extends PCGAgent

signal action_evaluated(action_data: ActionData)

@export var evaluation_interval := Vector2(10, 20)

@export_group("Traits")
@export_range(0.01, 1, 0.01) var survival := 1.0
@export_range(0, 1, 0.01) var thief := 0.0
@export_range(0, 1, 0.01) var lawful := 0.0
@export_range(0, 1, 0.01) var farmer := 0.0

@export_group("Resources")

@export_subgroup("Money")
@export_range(0, 1000, 1) var money_amount := 50
@export_range(0, 1, 0.01) var money_weight := 0.5

@export_subgroup("Food")
@export_range(0, 50, 1) var food_amount := 5
@export_range(0, 1, 0.01) var food_weight := 0.5

@export_subgroup("Satiation")
@export_range(0, 100, 1) var satiation_amount := 100
@export_range(0, 1, 0.01) var satiation_weight := 0.5

@export_subgroup("Companionship")
@export_range(0, 100, 1) var companionship_amount := 100
@export_range(0, 1, 0.01) var companionship_weight := 0.5

@export_subgroup("Duty")
@export_range(0, 100, 1) var duty_amount := 100
@export_range(0, 1, 0.01) var duty_weight := 0.5

func _ready():
	money_final = money_amount
	food_final = food_amount

func emit_action_evaluated(action_data: ActionData) -> void:
	action_evaluated.emit(action_data)