class_name Actor
extends CharacterBody2D

const HUNGER_DMG_INTERVAL := 2

@export var hit_points := 5
@export var max_speed := 40
@export var acceleration := 8
@export var friction := 4

var actors_in_range := {}
var hunger_dmg_cooldown := 0.0

@onready var hud = %HUDInterface
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
	PCG.satiation_depleted.connect(_on_satiation_depleted)

func _process(delta):
	hunger_dmg_cooldown -= delta
	if hunger_dmg_cooldown < 0:
		hunger_dmg_cooldown = 0

func set_blend_positions(x: float):
	animation_tree.set("parameters/Idle/blend_position", x)
	animation_tree.set("parameters/Move/blend_position", x)
	animation_tree.set("parameters/Eat/blend_position", x)
	animation_tree.set("parameters/Harvest/blend_position", x)
	animation_tree.set("parameters/Attack/blend_position", x)

func apply_damage(attacker: Actor = null):
	hit_points -= 1

	if hit_points <= 0:
		if attacker != null:
			var crime := Crime.new(Crime.Category.MURDER, attacker)
			WorldState.add_pending_crime(crime)
			PCG.emit_crime_committed(crime)
		else:
			hud.broadcast_event(name + " died from hunger")

		WorldState.unregister_actor(self)
		queue_free()
	elif attacker != null:
		PCG.emit_threat_present(attacker, self)
		handle_assault(attacker)
	else:
		hud.broadcast_event(name + " took damage from hunger")

func start_interaction(_target):
	pass

func stop_interaction():
	pass

func handle_detainment(detainer: Actor):
	detainer.carry_prop.set_texture(name)
	detainer.carry_prop.show_sprite()
	visible = false
	do_handle_detainment(detainer)

func handle_captivity(detainer: Actor, prison: Prison):
	detainer.carry_prop.hide_sprite()
	global_position = prison.global_position
	visible = true
	do_handle_captivity()

func do_handle_detainment(_detainer: Actor):
	pass

func do_handle_captivity():
	pass
	
func handle_crime_committed(_crime: Crime):
	pass

func handle_assault(_attacker: Actor):
	pass

func apply_knockback(_direction: Vector2, _force: float):
	pass

func actor_pressed():
	pass
	
func _on_hover_area_input_event(_viewport, event, _shape_idx):
	if event is InputEventMouseButton:
		if event.is_released() and event.button_index == MOUSE_BUTTON_RIGHT:
			actor_pressed()

func _on_animation_tree_animation_finished(_anim_name: StringName):
	pass

func _on_hurt_box_area_entered(area):
	var weapon = area as Weapon
	if weapon.actor == self: return

	var direction = weapon.global_position.direction_to(global_position)
	apply_knockback(direction, weapon.knockback)
	apply_damage(weapon.actor)

func _on_crime_committed(crime: Crime):
	if crime.criminal == self:
		return

	if crime.criminal in actors_in_range:
		crime.record_participant(self)
		handle_crime_committed(crime)

func _on_satiation_depleted(actor: Actor):
	if actor == self and hunger_dmg_cooldown <= 0:
		hunger_dmg_cooldown = HUNGER_DMG_INTERVAL
		apply_damage()
