class_name Memorizer
extends Node

@export var actor_owner: Actor

var actor_data: Dictionary[Actor, ActorData] = {}

func initialize(actors: Array) -> void:
	for actor in actors:
		var new_data = ActorData.new(self)
		new_data.last_known_position = actor.global_position
		actor_data[actor] = new_data

func is_friendly(actor: Actor) -> bool:
	return actor_data[actor].relationship >= 5

func is_trusted(actor: Actor) -> bool:
	return actor_data[actor].relationship >= 15

func is_close(actor: Actor) -> bool:
	return actor_data[actor].relationship >= 25

func get_relationship(actor: Actor) -> float:
	return actor_data[actor].relationship

func modify_relationship(actor: Actor, amount: int) -> void:
	actor_data[actor].relationship += amount

func get_last_known_position(actor: Actor) -> Vector2:
	return actor_data[actor].last_known_position

func set_last_known_position(actor: Actor, position: Vector2) -> void:
	actor_data[actor].last_known_position = position
