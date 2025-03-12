@tool
extends ActionLeaf

func tick(_actor: Node, blackboard: Blackboard) -> int:
	var crop_tiles = get_tree().get_nodes_in_group("CropTiles")
	
	for crop_tile in crop_tiles:
		var tile = crop_tile as CropTile
		if tile.status == CropTile.Status.MATURE and not tile.is_attended:
			tile.is_attended = true
			blackboard.set_value("crop_tile", tile)
			return SUCCESS
	
	return FAILURE