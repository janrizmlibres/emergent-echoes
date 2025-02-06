extends ActionLeaf

signal move_player(set_state: String, patrol_location: Vector2)

var current_patrol_index: int = 0

const patrol_locations = [Vector2(720, 48), Vector2(608, 112), Vector2(528, 16), Vector2(352, 48), Vector2(112, 208), Vector2(192, 352), Vector2(336, 416), Vector2(624, 416), Vector2(864, 416), Vector2(896, 160)]	
	
func tick(_actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("is_idle") == false:
		return FAILURE
	
	move_player.emit(patrol_locations[current_patrol_index])
		
	if _actor.navigation_agent_2d.is_navigation_finished():
		if current_patrol_index < patrol_locations.size() - 1:
			current_patrol_index += 1
		else:
			current_patrol_index = 0
		return SUCCESS
			
	return RUNNING
