@tool
extends ActionLeaf

enum Status {
	DORMANT = CropTile.Status.DORMANT,
	GROWING = CropTile.Status.GROWING
}

@export var status: Status = Status.DORMANT

func tick(_actor: Node, blackboard: Blackboard) -> int:
	var crop_tile = blackboard.get_value("crop_tile")
	crop_tile.status = status
	crop_tile.is_attended = false
	crop_tile.visible = true if status == Status.GROWING else false
	return SUCCESS