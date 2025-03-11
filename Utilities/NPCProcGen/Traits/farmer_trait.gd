class_name FarmerTrait
extends BaseTrait

func evaluation_proactive_action():
	var crop_tiles = get_tree().get_nodes_in_group("CropTiles")
	print("Crop Tiles: ", crop_tiles)
	evaluate_crop_status(crop_tiles, CropTile.Status.MATURE, Globals.Action.HARVEST)
	evaluate_crop_status(crop_tiles, CropTile.Status.DORMANT, Globals.Action.PLANT)

func evaluate_crop_status(crop_tiles, status, action):
	for tile in crop_tiles:
		if (tile as CropTile).status == status:
			var action_data = {"action": action, "data": {}}
			action_candidates.append({
				"action_data": action_data,
				"weight": weight
			})
			return