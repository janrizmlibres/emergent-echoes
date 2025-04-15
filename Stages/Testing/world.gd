extends Node2D

func _ready():
	var actors := get_tree().get_nodes_in_group("Actors")
	
	for actor in actors:
		WorldState.register_actor(actor)
