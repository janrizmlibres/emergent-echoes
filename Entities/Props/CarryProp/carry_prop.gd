class_name CarryProp
extends Node2D

@onready var carry_sprite: Sprite2D = $CarrySprite

func set_texture(actor_name: String) -> void:
	var texture_name = "res://Entities/Actors/"
	
	if actor_name == "Player":
		texture_name += "Player/Art/player.png"
	else:
		texture_name += "NPCs/" + actor_name + "/Art/" + actor_name.to_lower() + ".png"

	var texture = load(texture_name)
	carry_sprite.texture = texture

func show_sprite():
	print("Showing carry sprite")
	carry_sprite.visible = true

func hide_sprite():
	print("Hiding carry sprite")
	carry_sprite.visible = false
