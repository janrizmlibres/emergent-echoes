extends Node

class ActorState:
	var current_action: Globals.Action = Globals.Action.NONE
	var is_busy: bool = false
	var is_available: bool = true

var global_events: Array[String] = []
var actor_state: Dictionary[Actor, ActorState] = {}
var crimes: Array = [Crime]

var total_food: ResourceStat

func initialize(_total_food: ResourceStat, actors: Array) -> void:
	total_food = _total_food

	for actor in actors:
		var peer_actors = actors.duplicate()
		peer_actors.erase(actor)
		actor.memorizer.initialize(peer_actors)
		
		actor_state[actor] = ActorState.new()
		total_food.amount += actor.get_resource_amount(Globals.ResourceType.FOOD)

func get_peer_actors(actor: Actor) -> Array[Actor]:
	var peer_actors = actor_state.keys().duplicate()
	peer_actors.erase(actor)
	return peer_actors

func get_actor_count() -> int:
	return actor_state.size()

func get_current_action(actor):
	return actor_state[actor].current_action

func set_current_action(actor, action):
	actor_state[actor].current_action = action

func is_busy(actor):
	return actor_state[actor].is_busy

func set_is_busy(actor, value):
	actor_state[actor].is_busy = value

func is_available(actor):
	return actor_state[actor].is_available

func set_availability(actor, value):
	actor_state[actor].is_available = value

func is_actor_valid_target(initiator: Actor, target: Actor) -> bool:
	var target_last_position = initiator.memorizer.get_last_known_position(target)
	if target_last_position == Vector2.INF and not initiator.actors_in_range.has(target):
		return false

	return actor_state[target].is_available

func get_open_case() -> Crime:
	for crime in crimes:
		if crime.is_open():
			return crime
	return null
