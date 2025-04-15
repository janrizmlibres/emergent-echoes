@tool
extends Node2D

@export var angle_cone_of_vision := deg_to_rad(160.0)
@export var max_view_distance := 70.0
@export var angle_between_rays := deg_to_rad(5.0)

var npc_owner: NPC

func _ready():
	npc_owner = get_parent() as NPC
	
	if not npc_owner:
		print("VisionCone: No NPC owner found.")
		
	generate_raycasts()

func generate_raycasts() -> void:
	var ray_count := angle_cone_of_vision / angle_between_rays

	for index in ray_count:
		var ray := RayCast2D.new()
		var angle := angle_between_rays * (index - ray_count / 2.0)
		ray.target_position = Vector2.UP.rotated(deg_to_rad(90) + angle) * max_view_distance
		add_child(ray)
		ray.enabled = true

func _physics_process(_delta):
	if Engine.is_editor_hint() or not npc_owner:
		return

	var detected_actors = []

	for ray in get_children():
		ray = ray as RayCast2D

		if not ray.is_colliding(): continue

		var collider = ray.get_collider()
		if collider is Actor and collider not in detected_actors:
			detected_actors.append(collider)
	
	for actor in npc_owner.actors_in_range.keys():
		if actor not in detected_actors:
			WorldState.memory_manager.set_last_known_position(
				npc_owner,
				actor,
				actor.global_position
			)
			npc_owner.actors_in_range.erase(actor)

		detected_actors.erase(actor)

	for actor in detected_actors:
		npc_owner.actors_in_range[actor] = null
