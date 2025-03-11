@tool
extends ActionLeaf

func tick(_actor: Node, blackboard: Blackboard) -> int:
	var crop_tile = blackboard.get_value("crop_tile")
	crop_tile.status = CropTile.Status.DORMANT
	crop_tile.is_attended = false
	crop_tile.sprite.visible = false
	return SUCCESS