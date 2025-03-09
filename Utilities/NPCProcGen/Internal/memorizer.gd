class_name Memorizer
extends Node

@export var actor_owner: Actor

var actor_data: Dictionary[Actor, ActorData] = {}

func initialize(actors: Array) -> void:
	for actor in actors:
		actor_data[actor] = ActorData.new(self)

func is_friendly(actor: Actor) -> bool:
	return actor_data[actor].relationship >= 5

func is_trusted(actor: Actor) -> bool:
	return actor_data[actor].relationship >= 15

func is_close(actor: Actor) -> bool:
	return actor_data[actor].relationship >= 25

func get_last_known_position(actor: Actor) -> Vector2:
	return actor_data[actor].last_known_position
	
func is_valid_petition_target(actor: Actor, type: Globals.ResourceType) -> bool:
	if WorldState.actor_state[actor].current_petition_resource == type:
		return false
		
	return actor_data[actor].last_resource_petitioned != type
