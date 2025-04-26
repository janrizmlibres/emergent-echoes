@tool
extends ActionLeaf

enum Status {
	DORMANT = CropTile.Status.DORMANT,
	MATURE = CropTile.Status.MATURE
}

@export var status: Status = Status.DORMANT

func tick(actor: Node, blackboard: Blackboard) -> int:
	var crop_tile := WorldState.get_crop_in_status(status as int, actor)
	
	if crop_tile != null:
		crop_tile.is_attended = true
		blackboard.set_value("crop_tile", crop_tile)
		blackboard.set_value("move_position", crop_tile.global_position)
		return SUCCESS
	
	return FAILURE
