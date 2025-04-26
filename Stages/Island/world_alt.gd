extends Node2D

var prompt_scene: PackedScene = preload("res://Entities/UI/Overlays/Prompt/Alt/prompt_alt.tscn")
var player_petition: PackedScene = preload(
	"res://Entities/UI/Overlays/Petition/Alt/player_petition_prompt_alt.tscn"
)
var npc_petition: PackedScene = preload("res://Entities/UI/Overlays/Petition/petition_prompt.tscn")

func _ready():
	var actors := get_tree().get_nodes_in_group("Actors")

	for actor in actors:
		GameManager.register_actor(actor)

	EventManager.info_dialog_requested.connect(_on_info_dialog_requested)
	EventManager.show_player_petition_hud.connect(_on_show_petition_hud)
	EventManager.show_npc_petition_hud.connect(_on_show_npc_petition_hud)

func _on_info_dialog_requested(actor) -> void:
	var prompt_instance: PromptAlt = prompt_scene.instantiate()
	add_child(prompt_instance)
	prompt_instance.populate_data(actor)
	get_tree().paused = true

func _on_show_petition_hud(_target) -> void:
	var petition_instance = player_petition.instantiate()
	add_child(petition_instance)
	get_tree().paused = true

func _on_show_npc_petition_hud(npc_petitioner, resource_type, quantity) -> void:
	var petition_instance: PetitionPrompt = npc_petition.instantiate()
	add_child(petition_instance)
	petition_instance.initialize(npc_petitioner, resource_type, quantity)
	get_tree().paused = true
