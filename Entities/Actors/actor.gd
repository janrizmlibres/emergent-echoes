class_name Actor
extends CharacterBody2D

@export var hit_points: float = 1
@export var max_speed: int = 40
@export var acceleration: int = 8
@export var friction: int = 4

var resources: Array[ResourceStat] = []
var actors_in_range: Array[Actor] = []

@onready var memorizer: Memorizer = $Memorizer
@onready var seed_prop: Sprite2D = $SeedProp
@onready var actor_detector: Area2D = $ActorDetector
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

func holds_resource(type: Globals.ResourceType) -> bool:
	var resource = get_resource(type)
	return resource.amount > 0 if resource != null else false

func resource_deficient(type: Globals.ResourceType) -> bool:
	var resource = get_resource(type)
	assert(resource != null, "Actor must have resource")
	return resource.amount < resource.lower_threshold

func get_resource(type: Globals.ResourceType) -> ResourceStat:
	var idx = resources.find_custom(func(res: ResourceStat): return res.type == type)
	return resources[idx] if idx != -1 else null

func get_resource_amount(type: Globals.ResourceType) -> float:
	var resource = get_resource(type)
	return resource.amount if resource != null else 0

func modify_resource(type: Globals.ResourceType, amount: float):
	var resource = get_resource(type)
	if resource == null: return
	resource.amount += amount

func give_resource(receiver: Actor, resource_type: Globals.ResourceType, amount: float):
	var sender_resource = get_resource(resource_type)
	var receiver_resource = receiver.get_resource(resource_type)
	sender_resource.amount -= amount
	receiver_resource.amount += amount

func is_trackable(initiator: Actor) -> bool:
	var target_last_position = initiator.memorizer.get_last_known_position(self)
	if target_last_position != Vector2.INF: return true
	if initiator.actors_in_range.has(self): return true
	return false

func is_valid_target() -> bool:
	if is_queued_for_deletion(): return false
	if not WorldState.is_available(self): return false
	return not WorldState.is_captured(self)

func is_lawful() -> bool:
	if self is Player: return false
	return (self as NPC).lawful_trait != null

func start_interaction(_target):
	pass

func stop_interaction():
	pass

func _on_actor_detector_body_entered(body: Node2D):
	if body == self or body is not Actor: return
	actors_in_range.append(body as Actor)

func _on_actor_detector_body_exited(body: Node2D):
	if body == self or body is not Actor: return
	if body.is_queued_for_deletion(): return
	
	var actor = body as Actor
	memorizer.set_last_known_position(actor, actor.global_position)
	actors_in_range.erase(actor)

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
		(self as NPC).executor.procedural_tree.disable()

	WorldState.queue_free_actor(self)
	queue_free()
	
	if damager == null:
		Logger.info(name + " has perished!")
		return

	var participants = damager.actors_in_range.duplicate()
	var crime: Crime = Crime.new(Crime.Category.MURDER, damager, participants)
	WorldState.crimes.append(crime)

	Logger.info(damager.name + " murdered " + name)

	for participant in participants:
		if participant is NPC:
			(participant as NPC).crime_witnessed(crime)

func apply_knockback(_direction: Vector2, _force: float):
	pass
