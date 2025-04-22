class_name Actor
extends CharacterBody2D

@export var hit_points: float = 1
@export var max_speed: int = 40
@export var acceleration: int = 8
@export var friction: int = 4

var actors_in_range := {}

@onready var seed_prop: Sprite2D = $SeedProp
@onready var rear_marker: Marker2D = $RearMarker/RearMarker
@onready var carry_prop: CarryProp = $CarryProp

@onready var animation_tree: AnimationTree = $AnimationTree
@onready
var animation_state: AnimationNodeStateMachinePlayback = animation_tree.get("parameters/playback")

@onready var emote_bubble: EmoteBubble = $EmoteBubble
@onready var float_text_controller: FloatTextController = $FloatTextController

func _ready():
	animation_tree.active = true

func set_blend_positions(x: float):
	animation_tree.set("parameters/Idle/blend_position", x)
	animation_tree.set("parameters/Move/blend_position", x)
	animation_tree.set("parameters/Eat/blend_position", x)
	animation_tree.set("parameters/Harvest/blend_position", x)
	animation_tree.set("parameters/Attack/blend_position", x)

func start_interaction(_target):
	pass

func stop_interaction():
	pass

func _on_animation_tree_animation_finished(_anim_name: StringName):
	pass

func _on_hurt_box_area_entered(area):
	var weapon = area as Weapon
	if weapon.actor == self: return

	var direction = weapon.global_position.direction_to(global_position)
	apply_knockback(direction, weapon.knockback)
	apply_damage(weapon.actor)

func apply_damage(damager: Actor = null):
	hit_points -= 1
	if hit_points > 0: return

	if self is NPC:
		(self as NPC).executor.bt.disable()

	WorldState.queue_free_actor(self)
	queue_free()

	var participants = damager.actors_in_range.duplicate()
	var crime: Crime = Crime.new(Crime.Category.MURDER, damager, participants)
	WorldState._crimes.append(crime)

	for participant in participants:
		if participant is NPC:
			(participant as NPC).crime_witnessed(crime)

func apply_knockback(_direction: Vector2, _force: float):
	pass

func _on_hover_area_input_event(_viewport, event, _shape_idx):
	if event is InputEventMouseButton:
		if event.is_released() and event.button_index == MOUSE_BUTTON_RIGHT:
			actor_pressed()

func actor_pressed():
	pass
