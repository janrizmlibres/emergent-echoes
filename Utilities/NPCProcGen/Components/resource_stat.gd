class_name ResourceStat
extends Node

signal satiation_depleted

const TANGIBLE_TYPES = [Globals.ResourceType.MONEY, Globals.ResourceType.FOOD]
const INTEGER_TYPES = [
	Globals.ResourceType.TOTAL_FOOD,
	Globals.ResourceType.MONEY,
	Globals.ResourceType.FOOD
]
const DECAY_TYPES = [
	Globals.ResourceType.SATIATION,
	Globals.ResourceType.COMPANIONSHIP,
	Globals.ResourceType.DUTY
]
const PERCENT_TYPES = [
	Globals.ResourceType.SATIATION,
	Globals.ResourceType.COMPANIONSHIP,
	Globals.ResourceType.DUTY
]
const MAX_VALUE = {
	Globals.ResourceType.TOTAL_FOOD: Globals.INT_MAX,
	Globals.ResourceType.MONEY: 1000,
	Globals.ResourceType.FOOD: 50,
	Globals.ResourceType.SATIATION: 100,
	Globals.ResourceType.COMPANIONSHIP: 100,
	Globals.ResourceType.DUTY: 100
}
const LOCAL_THRESHOLDS = {
	Globals.ResourceType.MONEY: [100, 900],
	Globals.ResourceType.FOOD: [5, 45],
	Globals.ResourceType.SATIATION: [5, 90],
	Globals.ResourceType.COMPANIONSHIP: [10, 90],
	Globals.ResourceType.DUTY: [30, 90]
}

const DECAY_RATE: float = 0.2

@export var damage_interval: float = 3

@export var type: Globals.ResourceType = Globals.ResourceType.NONE
@export var amount: float = 0:
	get:
		return amount
	set(value):
		amount = clamp(value, 0, ResourceStat.MAX_VALUE[type])
		amount = floor(amount) if INTEGER_TYPES.has(type) else amount

		if type == Globals.ResourceType.SATIATION and amount > 0:
			damage_timer = 0

@export_range(0.01, 1, 0.01)

var weight: float = 0.5
var lower_threshold: int
var upper_threshold: int
var damage_timer: float = 0

func _ready():
	if LOCAL_THRESHOLDS.has(type):
		lower_threshold = LOCAL_THRESHOLDS[type][0]
		upper_threshold = LOCAL_THRESHOLDS[type][1]

func _process(delta):
	if type == Globals.ResourceType.SATIATION and amount == 0:
		damage_timer += delta

		if damage_timer >= damage_interval:
			damage_timer = 0
			satiation_depleted.emit()

	if not DECAY_TYPES.has(type): return
	var applied_decay = DECAY_RATE * delta

	if type == Globals.ResourceType.DUTY:
		amount += applied_decay if WorldState.crimes.is_empty() else -applied_decay
		return
	
	amount -= applied_decay

func is_unbounded() -> bool:
	return MAX_VALUE[type] == Globals.INT_MAX

func get_deficiency_point() -> float:
	return lerp(lower_threshold, upper_threshold - 1, weight)

func get_max_value() -> float:
	return MAX_VALUE[type]