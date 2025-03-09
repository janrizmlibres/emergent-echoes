class_name Memorizer
extends Node

var actor_data: Dictionary[Actor, ActorData] = {}

func initialize(actors: Array) -> void:
	for actor in actors:
		actor_data[actor] = ActorData.new(self)

func get_actor_relationship(actor: Actor) -> float:
	return actor_data[actor].relationship

func update_relationship(actor: Actor, amount: float) -> void:
	actor_data[actor].relationship += amount

func is_friendly(actor: Actor) -> bool:
	return actor_data[actor].relationship >= 5

func is_trusted(actor: Actor) -> bool:
	return actor_data[actor].relationship >= 15

func is_close(actor: Actor) -> bool:
	return actor_data[actor].relationship >= 25

func get_last_known_position(actor: Actor) -> Vector2:
	return actor_data[actor].last_known_position

func set_last_known_position(actor: Actor, location: Vector2) -> void:
	actor_data[actor].last_known_position = location

func set_last_petition_resource(actor: Actor, type: Globals.ResourceType) -> void:
	actor_data[actor].last_resource_petitioned = type
	
func is_valid_petition_target(actor: Actor, type: Globals.ResourceType) -> bool:
	if WorldState.actor_state[actor].current_petition_resource == type:
		return false
		
	return actor_data[actor].last_resource_petitioned != type
