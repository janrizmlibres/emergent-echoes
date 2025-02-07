extends ActionLeaf

signal move_player(set_state: String, patrol_location: Vector2)

var current_patrol_index: int = 0

const patrol_locations = [Vector2(464, 240), Vector2(448, 64), Vector2(288, 128), Vector2(112, 128), Vector2(432, 352), Vector2(768, 496), Vector2(912, 544), Vector2(848, 240), Vector2(832, 96)]
	
func tick(_actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") != "patrolling":
		return FAILURE
	
	move_player.emit(patrol_locations[current_patrol_index])
		
	if _actor.navigation_agent_2d.is_navigation_finished():
		if current_patrol_index < patrol_locations.size() - 1:
			current_patrol_index += 1
		else:
			current_patrol_index = 0
		return SUCCESS
			
	return RUNNING
