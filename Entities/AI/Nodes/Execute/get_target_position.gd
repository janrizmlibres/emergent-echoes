@tool
extends ActionLeaf

enum Waypoint {
	POINT,
	REAR,
	LATERAL,
	OMNI
}

@export var waypoint: Waypoint = Waypoint.LATERAL
@export var position_offset: float = 12

func tick(actor: Node, blackboard: Blackboard) -> int:
	var npc = actor as NPC
	var target: Actor = blackboard.get_value("target")

	var target_position: Vector2
	match waypoint:
		Waypoint.POINT:
			target_position = target.global_position
		Waypoint.REAR:
			target_position = target.rear_marker.global_position
		Waypoint.LATERAL:
			target_position = get_lateral_waypoint(npc.global_position, target.global_position)
		Waypoint.OMNI:
			target_position = get_omni_waypoint(npc.global_position, target.global_position)
		_:
			assert(false, "Invalid waypoint")

	blackboard.set_value("move_position", target_position)
	return SUCCESS

func get_lateral_waypoint(origin: Vector2, move_position: Vector2):
	var offset1: Vector2 = Vector2(position_offset, 0)
	var adjusted_position1: Vector2 = move_position + offset1
	var distance1: float = origin.distance_to(adjusted_position1)

	var offset2: Vector2 = Vector2((-position_offset), 0)
	var adjusted_position2: Vector2 = move_position + offset2
	var distance2: float = origin.distance_to(adjusted_position2)

	return adjusted_position1 if distance1 < distance2 else adjusted_position2

func get_omni_waypoint(origin: Vector2, move_position: Vector2):
	var direction = move_position.direction_to(origin)
	return move_position + direction * position_offset