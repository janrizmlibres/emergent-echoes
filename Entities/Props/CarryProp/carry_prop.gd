class_name CarryProp
extends Node2D

@onready var carry_sprite: Sprite2D = $CarrySprite

func set_texture(actor_name: String) -> void:
	var texture_name = "res://Entities/Actors/"
	
	if actor_name == "Player":
		texture_name += "Player/Art/player.png"
	else:
		var npc_name = get_npc_name(actor_name)
		texture_name += "NPCs/" + npc_name + "/Art/" + npc_name.to_lower() + ".png"

	var texture = load(texture_name)
	carry_sprite.texture = texture

func get_npc_name(npc_name: String) -> String:
	if npc_name.contains("Toblin"):
		return "Toblin"
	elif npc_name.contains("Silas"):
		return "Silas"
	elif npc_name.contains("Garreth"):
		return "Garreth"
	elif npc_name.contains("Pimble"):
		return "Pimble"
	else:
		return "Unknown"

func show_sprite():
	carry_sprite.visible = true

func hide_sprite():
	carry_sprite.visible = false
