@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	var crop_tile := WorldState.get_crop_in_status(CropTile.Status.MATURE, actor)

	if crop_tile != null:
		crop_tile.is_attended = true
		blackboard.set_value("crop_tile", crop_tile)
		return SUCCESS
	
	return FAILURE
