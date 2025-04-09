@tool
extends ActionLeaf

@export var max_radius: float = 150
@export var min_radius: float = 10
@export var centered: bool = false

var origin: Vector2 = Vector2.INF

func tick(actor: Node, blackboard: Blackboard) -> int:
	var actor_node = actor as NPC
	assert(actor_node != null, "Actor must be an NPC")

	if not centered or origin == Vector2.INF:
		origin = actor_node.global_position

	var random_position: Vector2 = get_point_in_circle()
	blackboard.set_value("move_position", random_position)
	
	return SUCCESS

func get_point_in_circle() -> Vector2:
	var angle = randf() * 2 * PI
	var random_radius = max_radius * sqrt(randf()) if min_radius == -1 \
		else lerp(min_radius, max_radius, sqrt(randf()))
	return origin + Vector2(cos(angle) * random_radius, sin(angle) * random_radius)
