extends Node2D

var prompt_scene: PackedScene = preload("res://Entities/UI/Overlays/Prompt/prompt.tscn")
var player_petition: PackedScene = preload(
	"res://Entities/UI/Overlays/Petition/player_petition_prompt.tscn"
)
var npc_petition: PackedScene = preload("res://Entities/UI/Overlays/Petition/petition_prompt.tscn")

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
	%HUDInterface.initialize()

	EventManager.info_dialog_requested.connect(_on_info_dialog_requested)
	EventManager.show_player_petition_hud.connect(_on_show_petition_hud)
	EventManager.show_npc_petition_hud.connect(_on_show_npc_petition_hud)

func _on_info_dialog_requested(actor: Actor) -> void:
	var prompt_instance: Prompt = prompt_scene.instantiate()
	add_child(prompt_instance)
	prompt_instance.populate_data(actor)
	get_tree().paused = true

func _on_show_petition_hud(target: Actor) -> void:
	var petition_instance = player_petition.instantiate()
	add_child(petition_instance)
	petition_instance.assign_target(target)
	get_tree().paused = true

func _on_show_npc_petition_hud(npc_petitioner: NPC, resource_type, quantity) -> void:
	var petition_instance: PetitionPrompt = npc_petition.instantiate()
	add_child(petition_instance)
	petition_instance.initialize(npc_petitioner, resource_type, quantity)
	get_tree().paused = true
