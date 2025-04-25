class_name PlayerPetitionPrompt
extends CanvasLayer

var quantity := 0
var resource_type := PCG.ResourceType.MONEY
var target: Actor

@onready var money_btn: TextureButton = $Prompt/MoneyorFood/Money
@onready var food_btn: TextureButton = $Prompt/MoneyorFood/Food
@onready var number_label: Label = $Prompt/ControlButtons/Number

func _ready():
	number_label.text = str(quantity)

func _input(event):
	if event is InputEventKey and event.pressed:
		if event.keycode == KEY_ENTER:
			accept_petition()
			unpause()
		elif event.keycode == KEY_ESCAPE:
			unpause()

func assign_target(actor: Actor):
	target = actor

func accept_petition():
	var player := WorldState.get_player()
	var result := PCG.execute_petition(player, target, resource_type, quantity)

	if result[0]:
		player.float_text_controller.show_float_text(
			resource_type,
			str(result[1]),
			true
		)
	else:
		player.float_text_controller.show_float_text(
			PCG.ResourceType.COMPANIONSHIP,
			str(result[2]),
			true
		)

	target.set_react_state(NPC.ReactState.NONE)

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
