extends ActionLeaf

signal move_actor(set_state: String, patrol_location: Vector2)

var current_patrol_index: int = 0

const patrol_locations = [Vector2(752, 192), Vector2(400, 176), Vector2(160, 272), Vector2(256, 480), Vector2(400, 432), Vector2(912, 576), Vector2(992, 448), Vector2(864, 240)]	

func before_run(actor: Node, blackboard: Blackboard) -> void:
	if blackboard.get_value("current_state") == "patrolling":
		move_actor.emit(patrol_locations[current_patrol_index])
	
func tick(_actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") != "patrolling":
		return FAILURE
		
	if blackboard.get_value("agent_arrived") == true:
		if current_patrol_index < patrol_locations.size() - 1:
			current_patrol_index += 1
		else:
			current_patrol_index = 0
		
		blackboard.set_value("current_state", "idle")
		blackboard.set_value("agent_arrived", false)
		return SUCCESS
			
	return RUNNING
