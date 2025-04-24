class_name NPCRelationship
extends VBoxContainer

@onready var sprite2d: Sprite2D = $Sprite2D
@onready var heartbar: HeartBar = $HeartBar

var player_texture = preload("res://Entities/Actors/Player/Art/player.png")
var toblin_texture = preload("res://Entities/Actors/NPCs/Toblin/Art/toblin.png")
var silas_texture = preload("res://Entities/Actors/NPCs/Silas/Art/silas.png")
var garreth_texture = preload("res://Entities/Actors/NPCs/Garreth/Art/garreth.png")
var pimble_texture = preload("res://Entities/Actors/NPCs/Pimble/Art/pimble.png")

func update_data(actor: Actor, peer: Actor):
	sprite2d.texture = get_texture(peer.name)
	heartbar.ChangeHealth(get_hearts_count(actor, peer))

func get_texture(actor_name: String) -> Texture2D:
	if actor_name == "Player":
		return player_texture
	elif actor_name.contains("Toblin"):
		return toblin_texture
	elif actor_name.contains("Silas"):
		return silas_texture
	elif actor_name.contains("Garreth"):
		return garreth_texture
	elif actor_name.contains("Pimble"):
		return pimble_texture

	return player_texture

func get_hearts_count(actor, peer):
	if WorldState.memory_manager.is_close(actor, peer):
		return 3
	elif WorldState.memory_manager.is_trusted(actor, peer):
		return 2
	elif WorldState.memory_manager.is_friendly(actor, peer):
		return 1
	else:
		return 0