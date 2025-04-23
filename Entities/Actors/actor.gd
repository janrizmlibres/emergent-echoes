class_name Actor
extends CharacterBody2D

@export var hit_points := 5
@export var max_speed := 40
@export var acceleration := 8
@export var friction := 4

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
	PCG.crime_committed.connect(_on_crime_committed)

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

func _on_crime_committed(crime: Crime):
	if crime.criminal != self and crime.criminal in actors_in_range:
		crime.record_participant(self)
	
	notify_npc_crime_committed(crime)

func notify_npc_crime_committed(_crime: Crime):
	pass

func apply_damage(damager: Actor = null):
	hit_points -= 1

	if hit_points <= 0:
		var crime := Crime.new(Crime.Category.MURDER, damager)
		WorldState.record_crime(crime)
		WorldState.unregister_actor(self)
		PCG.crime_committed.emit(crime)
		queue_free()
	else:
		PCG.danger_occured.emit(damager)

func apply_knockback(_direction: Vector2, _force: float):
	pass

func _on_hover_area_input_event(_viewport, event, _shape_idx):
	if event is InputEventMouseButton:
		if event.is_released() and event.button_index == MOUSE_BUTTON_RIGHT:
			actor_pressed()

func actor_pressed():
	pass
