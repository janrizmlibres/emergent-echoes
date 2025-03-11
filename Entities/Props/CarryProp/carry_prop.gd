class_name CarryProp
extends Node2D

@onready var carry_sprite: Sprite2D = $CarrySprite

func set_texture(actor_name: String) -> void:
	var texture_name = "res://Entities/Actors/NPCs/" + actor_name + "/Art/" \
		+ actor_name.to_lower() + ".png"

	var texture = load(texture_name)
	carry_sprite.texture = texture

func show_sprite():
	carry_sprite.visible = true

func hide_sprite():
	carry_sprite.visible = false