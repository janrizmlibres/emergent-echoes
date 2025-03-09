class_name FarmerTrait
extends BaseTrait

func evaluation_proactive_action():
  var crops_tiles = get_tree().get_nodes_in_group("CropTiles")
  evaluate_crop_status(crops_tiles, CropTile.Status.MATURE, Globals.Action.HARVEST)
  evaluate_crop_status(crops_tiles, CropTile.Status.DORMANT, Globals.Action.PLANT)

func evaluate_crop_status(crop_tiles, status, action):
  for tile in crop_tiles:
    if (tile as CropTile).status == status:
      add_action(action, Globals.ResourceType.TOTAL_FOOD)
      return