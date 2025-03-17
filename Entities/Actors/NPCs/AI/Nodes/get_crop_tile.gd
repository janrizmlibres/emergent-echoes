@tool
extends ActionLeaf

enum Status {
	DORMANT = CropTile.Status.DORMANT,
	MATURE = CropTile.Status.MATURE
}

@export var status: Status = Status.DORMANT

func tick(_actor: Node, blackboard: Blackboard) -> int:
	var crop_tiles = get_tree().get_nodes_in_group("CropTiles")
	
	for crop_tile in crop_tiles:
		var tile = crop_tile as CropTile
		if tile.status == status and not tile.is_attended:
			tile.is_attended = true
			blackboard.set_value("crop_tile", tile)
			blackboard.set_value("move_position", tile.global_position)
			return SUCCESS
	
	return FAILURE