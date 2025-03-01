extends ActionLeaf

signal move_actor(patrol_location: Vector2)

var current_patrol_index: int = 0

const patrol_locations = [Vector2(512, 384), Vector2(512, 192), Vector2(384, 288), Vector2(176, 336), Vector2(496, 448), Vector2(736, 448), Vector2(864, 640), Vector2(960, 416), Vector2(672, 384)]
	
func tick(_actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") != "patrolling":
		return FAILURE
	
	blackboard.set_value("last_patrol_location", patrol_locations[current_patrol_index])
	move_actor.emit(patrol_locations[current_patrol_index])
		
	if _actor.navigation_agent_2d.is_navigation_finished():
		if current_patrol_index < patrol_locations.size() - 1:
			current_patrol_index += 1
		else:
			current_patrol_index = 0
		return SUCCESS
			
	return RUNNING
