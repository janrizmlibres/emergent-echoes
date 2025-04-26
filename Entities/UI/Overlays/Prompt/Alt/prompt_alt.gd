class_name PromptAlt
extends CanvasLayer

var npc_icon_scene: PackedScene = preload("res://Entities/UI/Overlays/Prompt/Alt/npc_relationship_alt.tscn")

var descriptions = {
	"Player": "You. Yes you! This is you. You control this character.",
	"Toblin": "Tobin is a plain-spoken villager with a knack for blending in. 
		Quiet and unassuming, he's the one folks turn to for local gossip or 
		a helping hand.",
	"Silas": "Silas is a nimble thief and master of stealth. Charming yet elusive, 
		he's known for slipping through shadows and pockets alike. 
		A wanted man with a hidden heart of gold.",
	"Garreth": "Garreth is a stern village law enforcer with a sharp eye for trouble. 
		Gruff but fair, he patrols with a steady hand, keeping order and criminals in check.",
	"Pimble": "Pimble is a sprightly gnome farmer with a love for tiny harvests. 
		Cheerful and clever, he tends his crops with gnomish ingenuity and a mischievous grin."
}

@onready var npc_name: Label = $Prompt/VBoxContainer/NPCName
@onready var npc_description: Label = $Prompt/VBoxContainer/NPCDescription
@onready var traits_list: Label = $Prompt/VBoxContainer/HBoxContainer/Traits
@onready var relationships: HBoxContainer = $Prompt/VBoxContainer/ScrollContainer/HBoxContainer

func _input(event):
	if event is InputEventKey and event.pressed:
		if event.keycode == KEY_ESCAPE:
			queue_free()
			get_tree().paused = false
		
func populate_data(actor: CharacterBody2D):
	npc_name.text = actor.name
	npc_description.text = get_description(actor.name)
	traits_list.text = "No Traits"
	set_relationships(actor)

func set_relationships(actor: CharacterBody2D):
	var peers := GameManager.get_peers(actor)
	for peer in peers:
		if peer == null:
			continue
		var npc_icon: NPCRelationshipAlt = npc_icon_scene.instantiate()
		relationships.add_child(npc_icon)
		npc_icon.update_data(actor, peer)

func get_description(actor_name: String) -> String:
	if actor_name == "PlayerAlt":
		return descriptions["Player"]
	elif actor_name.contains("Toblin"):
		return descriptions["Toblin"]
	elif actor_name.contains("Silas"):
		return descriptions["Silas"]
	elif actor_name.contains("Garreth"):
		return descriptions["Garreth"]
	elif actor_name.contains("Pimble"):
		return descriptions["Pimble"]

	return "No Description"
