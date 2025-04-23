extends Node2D

func _ready():
	var actors := get_tree().get_nodes_in_group("Actors")
	var crop_tiles := get_tree().get_nodes_in_group("CropTiles")
	var prisons := get_tree().get_nodes_in_group("Prisons")
	
	for actor in actors:
		WorldState.register_actor(actor)
	
	for crop in crop_tiles:
		WorldState.register_crop(crop)
	
	for prison in prisons:
		WorldState.register_prison(prison)
	
	WorldState.register_shop($Shop)
