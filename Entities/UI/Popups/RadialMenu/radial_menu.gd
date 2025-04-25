class_name RadialMenu
extends Control

const DURATION := 4.0

@export var radius := 20
@export var speed := 0.25

var actor: Actor

var num: int
var active := false
var disabled := false

var timer := DURATION

@onready var petition_button: TextureButton = $PetitionButton

func _ready():
	hide()
	num = get_child_count()
	actor = get_parent()

func _process(delta):
	if not active:
		return

	timer -= delta

	if timer <= 0:
		timer = DURATION
		hide_menu()
	
func _unhandled_input(event):
	if event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_LEFT and event.pressed:
			accept_event()

func toggle():
	if disabled:
		return

	disabled = true
	if active:
		hide_menu()
	else:
		show_menu()

func show_menu():
	show()
	var spacing := TAU / num
	active = true
	timer = DURATION
	var tw := create_tween().set_parallel()

	for b in get_children():
		# Subtract PI/2 to align the first button to the top
		var a = spacing * b.get_index() - PI / 2
		var dest = Vector2(radius, 0).rotated(a)
		
		tw.tween_property(b, "position", dest, speed).from(
			Vector2.ZERO
		).set_trans(Tween.TRANS_BACK).set_ease(Tween.EASE_OUT)

		tw.tween_property(b, "scale", Vector2.ONE, speed).from(
			Vector2(0.5, 0.5)
		).set_trans(Tween.TRANS_LINEAR)
	
	tw.finished.connect(_on_tween_finished)

func hide_menu():
	active = false
	var tw = create_tween().set_parallel()
	tw.finished.connect(_on_tween_finished)

	for b in get_children():
		tw.tween_property(b, "position", Vector2.ZERO, speed).set_trans(
			Tween.TRANS_BACK
		).set_ease(Tween.EASE_IN)
		tw.tween_property(b, "scale", Vector2(0.5, 0.5), speed).set_trans(Tween.TRANS_LINEAR)

func enable_petition():
	petition_button.disabled = false

func disable_petition():
	petition_button.disabled = true

func _on_tween_finished():
	disabled = false
	if not active:
		hide()

func _on_info_button_button_up():
	EventManager.emit_info_dialog_requested(actor)

func _on_petition_button_button_up():
	EventManager.emit_show_player_petition_hud(actor)
