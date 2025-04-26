class_name PlayerPetitionPromptAlt
extends CanvasLayer

var player: PlayerAlt = null
var quantity := 0
var resource_type := PCG.ResourceType.MONEY

@onready var money_btn: TextureButton = $Prompt/MoneyorFood/Money
@onready var food_btn: TextureButton = $Prompt/MoneyorFood/Food
@onready var number_label: Label = $Prompt/ControlButtons/Number

func _ready():
	player = get_tree().get_first_node_in_group("Player")
	number_label.text = str(quantity)

func _input(event):
	if event is InputEventKey and event.pressed:
		if event.keycode == KEY_ENTER:
			accept_petition()
			unpause()
		elif event.keycode == KEY_ESCAPE:
			unpause()

func accept_petition():
	var is_accepted: bool

	if quantity > 100 and resource_type == PCG.ResourceType.MONEY:
		is_accepted = false
	elif quantity > 10 and resource_type == PCG.ResourceType.FOOD:
		is_accepted = false
	else:
		is_accepted = true if randf() < 0.5 else false

	if is_accepted:
		if resource_type == PCG.ResourceType.MONEY:
			player.money += quantity
		else:
			player.food += quantity

		player.float_text_controller.show_float_text(
			resource_type,
			str(quantity),
			true
		)
	else:
		player.float_text_controller.show_float_text(
			PCG.ResourceType.COMPANIONSHIP,
			"-2",
			true
		)

func unpause():
	queue_free()
	get_tree().paused = false

func _on_money_pressed():
	resource_type = PCG.ResourceType.MONEY
	money_btn.button_pressed = true
	food_btn.button_pressed = false

func _on_food_pressed():
	resource_type = PCG.ResourceType.FOOD
	food_btn.button_pressed = true
	money_btn.button_pressed = false

func _on_plus_pressed():
	quantity = min(99, quantity + 1)
	number_label.text = str(quantity)

func _on_minus_pressed():
	quantity = max(0, quantity - 1)
	number_label.text = str(quantity)
