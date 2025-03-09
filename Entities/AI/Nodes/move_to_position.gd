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

	var move_position: Vector2 = blackboard.get_value("move_position")
	var target_position: Vector2

	match waypoint:
		Waypoint.POINT:
			target_position = move_position
		Waypoint.REAR:
			target_position = get_rear_waypoint(npc)
		Waypoint.LATERAL:
			target_position = get_lateral_waypoint(npc.global_position, move_position)
		Waypoint.OMNI:
			target_position = get_omni_waypoint(npc.global_position, move_position)
		_:
			assert(false, "Invalid waypoint")

	npc.navigation_agent.target_position = target_position

	if (npc.navigation_agent.is_navigation_finished()):
		return SUCCESS
	
	npc.move_agent()
	return RUNNING

func get_rear_waypoint(npc: NPC):
	return npc.global_position.direction_to(npc.rear_marker.global_position) * position_offset

func get_lateral_waypoint(origin: Vector2, move_position: Vector2):
	var offset1: Vector2 = Vector2(position_offset, 0)
	var adjusted_position1: Vector2 = move_position + offset1
	var distance1: float = origin.distance_to(adjusted_position1)

	var offset2: Vector2 = Vector2((- position_offset), 0)
	var adjusted_position2: Vector2 = move_position + offset2
	var distance2: float = origin.distance_to(adjusted_position2)

	return adjusted_position1 if distance1 < distance2 else adjusted_position2

func get_omni_waypoint(origin: Vector2, move_position: Vector2):
	var direction = move_position.direction_to(origin)
	return move_position + direction * position_offset