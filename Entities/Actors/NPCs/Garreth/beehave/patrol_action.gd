extends ActionLeaf

signal on_patrol(patrol_location: Vector2)

var current_patrol_index: int = 0

const patrol_locations = [Vector2(464, 240), Vector2(448, 64), Vector2(288, 128), Vector2(112, 128), Vector2(432, 352), Vector2(768, 496), Vector2(912, 544), Vector2(848, 240), Vector2(832, 96)]

func tick(_actor: Node, _blackboard: Blackboard) -> int:
	if current_patrol_index >= patrol_locations.size():
		current_patrol_index = 0
		on_patrol.emit(patrol_locations[current_patrol_index])
		current_patrol_index += 1
	else:
		on_patrol.emit(patrol_locations[current_patrol_index])
		current_patrol_index += 1
	return SUCCESS
